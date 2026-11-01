using System.Runtime.CompilerServices;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Briefing;

/// <param name="Text">The timestamped text that prompts should work from.</param>
/// <param name="IsNotes">Whether the text is notes taken from a long transcript rather than the transcript itself.</param>
public sealed record TranscriptMaterial(string Text, bool IsNotes);

/// <summary>Gives prompts the transcript itself or, when it is too long for one request, notes taken from it. Notes are made once per transcript instance.</summary>
public sealed class TranscriptMaterialSource
{
    public const int MAX_WHOLE_TRANSCRIPT_WORDS = 30000;

    public TranscriptMaterialSource(ILanguageModel languageModel, BriefingPrompts prompts)
    {
        this.languageModel = languageModel;
        this.prompts = prompts;
    }

    public async Task<TranscriptMaterial> GetAsync(Transcript transcript, CancellationToken cancellationToken)
    {
        if (transcript.WordCount <= MAX_WHOLE_TRANSCRIPT_WORDS)
        {
            return new TranscriptMaterial(formatter.Format(transcript), false);
        }

        var notes = notesByTranscript.GetValue(transcript, key => new Lazy<Task<string>>(() => TakeNotesAsync(key, cancellationToken)));
        try
        {
            return new TranscriptMaterial(await notes.Value.ConfigureAwait(false), true);
        }
        catch
        {
            notesByTranscript.Remove(transcript);
            throw;
        }
    }

    //

    private readonly ILanguageModel languageModel;
    private readonly BriefingPrompts prompts;
    private readonly TranscriptFormatter formatter = new();
    private readonly TranscriptChunker chunker = new();
    private readonly ConditionalWeakTable<Transcript, Lazy<Task<string>>> notesByTranscript = new();

    private async Task<string> TakeNotesAsync(Transcript transcript, CancellationToken cancellationToken)
    {
        var chunks = chunker.Split(transcript);
        var notes = new List<string>();
        for (var index = 0; index < chunks.Count; index++)
        {
            var prompt = prompts.ForChunkNotes(formatter.Format(chunks[index]), index + 1, chunks.Count);
            var chunkNotes = await languageModel.CompleteAsync(prompt.System, prompt.User, false, cancellationToken).ConfigureAwait(false);
            notes.Add($"Notes for part {index + 1} of {chunks.Count}:\n{chunkNotes.Trim()}");
        }

        return string.Join("\n\n", notes);
    }
}