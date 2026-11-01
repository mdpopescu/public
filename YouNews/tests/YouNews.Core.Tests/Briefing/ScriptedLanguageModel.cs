using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Briefing;

public sealed record LanguageModelCall(string System, string User, bool Thinking);

/// <summary>Answers from a queue and records every prompt. When the queue is empty it repeats the fallback.</summary>
public sealed class ScriptedLanguageModel : ILanguageModel
{
    public List<LanguageModelCall> Calls { get; } = [];

    public string Fallback { get; set; } = "fallback answer";

    /// <summary>Every answer waits for this, so that a test can hold callers in flight.</summary>
    public Task Gate { get; set; } = Task.CompletedTask;

    public ScriptedLanguageModel(params string[] answers)
    {
        foreach (var answer in answers)
        {
            responses.Enqueue(() => answer);
        }
    }

    public void EnqueueFailure(Exception exception) => responses.Enqueue(() => throw exception);

    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, bool thinking, CancellationToken cancellationToken)
    {
        lock (responses)
        {
            Calls.Add(new LanguageModelCall(systemPrompt, userPrompt, thinking));
        }

        await Gate;
        lock (responses)
        {
            return responses.TryDequeue(out var response) ? response() : Fallback;
        }
    }

    //

    private readonly Queue<Func<string>> responses = new();
}