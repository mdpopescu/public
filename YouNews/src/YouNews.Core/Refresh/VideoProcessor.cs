using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Refresh;

public sealed class VideoProcessor : IVideoProcessor, IEntryReprocessor, IDisposable
{
    public const int RELATED_CANDIDATES = 100;

    public VideoProcessor(VideoProcessorDependencies dependencies)
    {
        this.dependencies = dependencies;
    }

    public async Task ProcessAsync(TrackedVideo video, ProcessingContext context, CancellationToken cancellationToken)
    {
        await turn.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await CreateEntryAsync(video, context, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            turn.Release();
        }
    }

    public async Task ReprocessAsync(ReprocessJob job, SetupSnapshot? setup, IReadOnlyList<string> interests, CancellationToken cancellationToken)
    {
        await turn.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await ReviseEntryAsync(job, setup, interests, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            turn.Release();
        }
    }

    public void Dispose() => turn.Dispose();

    //

    private readonly VideoProcessorDependencies dependencies;
    private readonly SemaphoreSlim turn = new(1, 1);

    private async Task CreateEntryAsync(TrackedVideo video, ProcessingContext context, CancellationToken cancellationToken)
    {
        var channelName = context.Channels.TryGetValue(video.ChannelId, out var channel) ? channel.Name : "Unknown channel";
        var day = await GetEntryDayAsync(video, cancellationToken).ConfigureAwait(false);
        var input = new GenerationInput(video.YouTubeId, video.Title, channelName, null, context.Setup, context.Interests);
        var generated = await GenerateAsync(input, cancellationToken).ConfigureAwait(false);
        var images = await dependencies.Images.CaptureAsync(video.YouTubeId, generated.Analysis.FrameTimes, cancellationToken).ConfigureAwait(false);

        var analysis = generated.Analysis;
        var entry = new NewEntry(video.Id, day, analysis.Headline, analysis.Tags, generated.Summary, generated.Opinion, generated.Sources, images, generated.RelatedEntryIds);
        await dependencies.Entries.AddAsync(entry, cancellationToken).ConfigureAwait(false);
    }

    private async Task ReviseEntryAsync(ReprocessJob job, SetupSnapshot? setup, IReadOnlyList<string> interests, CancellationToken cancellationToken)
    {
        var input = new GenerationInput(job.YouTubeId, job.Title, job.ChannelName, job.EntryId, setup, interests);
        var generated = await GenerateAsync(input, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(generated.Summary) || string.IsNullOrWhiteSpace(generated.Opinion))
        {
            throw new InvalidOperationException("The model returned an empty summary or opinion.");
        }

        var revision = new EntryRevision(job.EntryId, generated.Summary, generated.Opinion, generated.Sources, generated.RelatedEntryIds);
        await dependencies.Entries.ReviseAsync(revision, cancellationToken).ConfigureAwait(false);
    }

    private async Task<GeneratedTexts> GenerateAsync(GenerationInput input, CancellationToken cancellationToken)
    {
        var transcript = await GetTranscriptAsync(input.YouTubeId, cancellationToken).ConfigureAwait(false);
        var recent = await dependencies.Entries.ListRecentDigestsAsync(RELATED_CANDIDATES, cancellationToken).ConfigureAwait(false);
        var candidates = recent.Where(candidate => candidate.Id != input.ExcludedEntryId).ToList();

        var analysis = await dependencies.Writer.AnalyzeAsync(input.Title, input.ChannelName, transcript, candidates, cancellationToken).ConfigureAwait(false);
        var summary = await dependencies.Writer.SummarizeAsync(input.Title, transcript, cancellationToken).ConfigureAwait(false);
        var searchResults = await SearchAsync(analysis.SearchQueries, cancellationToken).ConfigureAwait(false);
        var related = candidates.Where(candidate => analysis.RelatedEntryIds.Contains(candidate.Id)).ToList();

        var opinionRequest = new OpinionRequest(input.Title, input.ChannelName, transcript, input.Setup, input.Interests, related, searchResults);
        var opinion = await dependencies.Writer.WriteOpinionAsync(opinionRequest, cancellationToken).ConfigureAwait(false);

        var sources = searchResults.Select(result => new WebSource(result.Title, result.Url)).ToList();
        return new GeneratedTexts(analysis, summary, opinion, sources, related.Select(candidate => candidate.Id).ToList());
    }

    /// <summary>Asks for the exact publication date, because the date stored from a channel listing may be rounded.</summary>
    private async Task<DateOnly> GetEntryDayAsync(TrackedVideo video, CancellationToken cancellationToken)
    {
        var details = await dependencies.Catalog.GetDetailsAsync(video.YouTubeId, cancellationToken).ConfigureAwait(false);
        return details.PublishedOn
            ?? video.PublishedOn
            ?? DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(video.FirstSeenAt, dependencies.Time.LocalTimeZone).DateTime);
    }

    private async Task<Transcript> GetTranscriptAsync(string youTubeId, CancellationToken cancellationToken)
    {
        var captions = await dependencies.Captions.GetCaptionsAsync(youTubeId, cancellationToken).ConfigureAwait(false);
        if (captions is { Segments.Count: > 0 })
        {
            return captions;
        }

        return await dependencies.Speech.TranscribeAsync(youTubeId, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<WebResult>> SearchAsync(IReadOnlyList<string> queries, CancellationToken cancellationToken)
    {
        var results = new List<WebResult>();
        foreach (var query in queries)
        {
            results.AddRange(await dependencies.Search.SearchAsync(query, cancellationToken).ConfigureAwait(false));
        }

        return results.DistinctBy(result => result.Url).ToList();
    }

    private sealed record GenerationInput(
        string YouTubeId,
        string Title,
        string ChannelName,
        long? ExcludedEntryId,
        SetupSnapshot? Setup,
        IReadOnlyList<string> Interests);

    private sealed record GeneratedTexts(
        VideoAnalysis Analysis,
        string Summary,
        string Opinion,
        IReadOnlyList<WebSource> Sources,
        IReadOnlyList<long> RelatedEntryIds);
}

/// <summary>The services needed to turn a video into an entry.</summary>
public sealed record VideoProcessorDependencies(
    IVideoCatalog Catalog,
    ICaptionSource Captions,
    ISpeechTranscriber Speech,
    IBriefingWriter Writer,
    IWebSearch Search,
    IImageSource Images,
    IEntryStore Entries,
    TimeProvider Time);