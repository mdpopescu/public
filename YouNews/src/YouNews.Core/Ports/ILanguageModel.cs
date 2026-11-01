namespace YouNews.Core.Ports;

/// <summary>Answers prompts with a language model.</summary>
public interface ILanguageModel
{
    /// <summary>Gets the model's visible answer, without any reasoning text. Throws when the model gives no usable answer.</summary>
    /// <param name="thinking">Whether the model may reason first, which is slower but gives better judgment.</param>
    Task<string> CompleteAsync(string systemPrompt, string userPrompt, bool thinking, CancellationToken cancellationToken);
}