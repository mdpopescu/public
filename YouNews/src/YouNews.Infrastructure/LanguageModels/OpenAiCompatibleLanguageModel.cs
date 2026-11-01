using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.LanguageModels;

/// <summary>Talks to an OpenAI-compatible server. A llama-swap in front may fail while it swaps models, hence the single retry.</summary>
public sealed class OpenAiCompatibleLanguageModel : ILanguageModel
{
    public OpenAiCompatibleLanguageModel(HttpClient httpClient, IOptions<YouNewsOptions> options, ICurrentModel currentModel, TimeProvider timeProvider)
    {
        this.httpClient = httpClient;
        this.options = options.Value.LanguageModel;
        this.currentModel = currentModel;
        this.timeProvider = timeProvider;
        httpClient.Timeout = TimeSpan.FromMinutes(this.options.TimeoutMinutes);
    }

    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, bool thinking, CancellationToken cancellationToken)
    {
        var model = await currentModel.GetAsync(cancellationToken).ConfigureAwait(false);
        var body = BuildRequestBody(model, systemPrompt, userPrompt, thinking);
        try
        {
            return await SendAsync(body, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException exception) when (exception.StatusCode is null or >= HttpStatusCode.InternalServerError)
        {
            await Task.Delay(RETRY_DELAY, timeProvider, cancellationToken).ConfigureAwait(false);
        }

        return await SendAsync(body, cancellationToken).ConfigureAwait(false);
    }

    //

    private const int MAX_ERROR_BODY_CHARACTERS = 300;

    private static readonly TimeSpan RETRY_DELAY = TimeSpan.FromSeconds(10);

    private readonly HttpClient httpClient;
    private readonly LanguageModelOptions options;
    private readonly ICurrentModel currentModel;
    private readonly TimeProvider timeProvider;

    private static string ReadContent(string responseBody)
    {
        using var document = ParseResponse(responseBody);
        if (!document.RootElement.TryGetProperty("choices", out var choices) || choices.ValueKind != JsonValueKind.Array || choices.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("The language model's response has no choices: " + Shorten(responseBody));
        }

        var choice = choices[0];
        var content = choice.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var contentElement) && contentElement.ValueKind == JsonValueKind.String
            ? contentElement.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(content))
        {
            throw EmptyContentException(choice);
        }

        if (choice.TryGetProperty("finish_reason", out var finishReason) && finishReason.ToString() == "length")
        {
            throw new InvalidOperationException("The language model's answer was cut off with finish_reason length: the token budget ran out. Raise MaxTokens.");
        }

        return content;
    }

    private static JsonDocument ParseResponse(string responseBody)
    {
        try
        {
            return JsonDocument.Parse(responseBody);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("The language model's response is not JSON: " + Shorten(responseBody), exception);
        }
    }

    private static InvalidOperationException EmptyContentException(JsonElement choice)
    {
        var finishReason = choice.TryGetProperty("finish_reason", out var reason) ? reason.ToString() : "missing";
        var message = finishReason == "length"
            ? "The language model returned empty content with finish_reason length: the token budget ran out, probably during reasoning. Raise MaxTokens."
            : $"The language model returned empty content with finish_reason {finishReason}.";

        return new InvalidOperationException(message);
    }

    private static string Shorten(string text) =>
        text.Length <= MAX_ERROR_BODY_CHARACTERS ? text : text[..MAX_ERROR_BODY_CHARACTERS] + "...";

    private string BuildRequestBody(string model, string systemPrompt, string userPrompt, bool thinking)
    {
        var body = new JsonObject
        {
            ["model"] = model,
            ["max_tokens"] = options.MaxTokens,
            ["messages"] = new JsonArray(
                new JsonObject { ["role"] = "system", ["content"] = systemPrompt },
                new JsonObject { ["role"] = "user", ["content"] = userPrompt }),
        };

        if (!thinking)
        {
            body["chat_template_kwargs"] = new JsonObject { ["enable_thinking"] = false };
        }

        return body.ToJsonString();
    }

    private async Task<string> SendAsync(string body, CancellationToken cancellationToken)
    {
        var url = options.BaseUrl.TrimEnd('/') + "/chat/completions";
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        try
        {
            using var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"The language model at {url} answered {(int)response.StatusCode} {response.StatusCode}: {Shorten(responseBody)}", null, response.StatusCode);
            }

            return ReadContent(responseBody);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The language model at {url} did not answer within {options.TimeoutMinutes} minutes.", exception);
        }
    }
}