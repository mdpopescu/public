using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Briefing;

public sealed class BriefingWriter : IBriefingWriter
{
    public const int MINIMUM_OPINION_PARAGRAPHS = 3;

    public BriefingWriter(ILanguageModel languageModel, TimeProvider timeProvider)
    {
        this.languageModel = languageModel;
        this.timeProvider = timeProvider;
        materials = new TranscriptMaterialSource(languageModel, prompts);
    }

    public async Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken)
    {
        var material = await materials.GetAsync(transcript, cancellationToken).ConfigureAwait(false);
        var prompt = prompts.ForAnalysis(title, channelName, material, candidates);
        var response = await CompleteAsync(prompt, false, cancellationToken).ConfigureAwait(false);
        try
        {
            return analysisParser.Parse(response, candidates, transcript);
        }
        catch (FormatException)
        {
            var retry = await CompleteAsync(prompts.RetryAnalysis(prompt), false, cancellationToken).ConfigureAwait(false);
            return analysisParser.Parse(retry, candidates, transcript);
        }
    }

    public async Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken)
    {
        var material = await materials.GetAsync(transcript, cancellationToken).ConfigureAwait(false);
        var targetWords = summaryLength.TargetWords(transcript.WordCount);
        var response = await CompleteAsync(prompts.ForSummary(title, material, targetWords), false, cancellationToken).ConfigureAwait(false);
        return cleaner.Clean(response);
    }

    public async Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken)
    {
        var material = await materials.GetAsync(request.Transcript, cancellationToken).ConfigureAwait(false);
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var prompt = prompts.ForOpinion(request, material, today);
        var opinion = cleaner.Clean(await CompleteAsync(prompt, true, cancellationToken).ConfigureAwait(false));
        if (CountParagraphs(opinion) >= MINIMUM_OPINION_PARAGRAPHS)
        {
            return opinion;
        }

        return cleaner.Clean(await CompleteAsync(prompts.RetryOpinion(prompt), true, cancellationToken).ConfigureAwait(false));
    }

    //

    private readonly ILanguageModel languageModel;
    private readonly TimeProvider timeProvider;
    private readonly TranscriptMaterialSource materials;
    private readonly BriefingPrompts prompts = new();
    private readonly SummaryLength summaryLength = new();
    private readonly PlainTextCleaner cleaner = new();
    private readonly AnalysisParser analysisParser = new();

    private static int CountParagraphs(string text) =>
        text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;

    private Task<string> CompleteAsync(Prompt prompt, bool thinking, CancellationToken cancellationToken) =>
        languageModel.CompleteAsync(prompt.System, prompt.User, thinking, cancellationToken);
}