using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Refresh;

public sealed class VideoProcessor : IVideoProcessor
{
    public const int RELATED_CANDIDATES = 100;

    public VideoProcessor(VideoProcessorDependencies dependencies)
    {
        this.dependencies = dependencies;
    }

    public async Task ProcessAsync(TrackedVideo video, ProcessingContext context, CancellationToken cancellationToken)
    {
        var channelName = context.Channels.TryGetValue(video.ChannelId, out var channel) ? channel.Name : "Unknown channel";
        var transcript = await GetTranscriptAsync(video.YouTubeId, cancellationToken).ConfigureAwait(false);
        var candidates = await dependencies.Entries.ListRecentDigestsAsync(RELATED_CANDIDATES, cancellationToken).ConfigureAwait(false);

        var analysis = await dependencies.Writer.AnalyzeAsync(video.Title, channelName, transcript, candidates, cancellationToken).ConfigureAwait(false);
        var summary = await dependencies.Writer.SummarizeAsync(video.Title, transcript, cancellationToken).ConfigureAwait(false);
        var searchResults = await SearchAsync(analysis.SearchQueries, cancellationToken).ConfigureAwait(false);
        var related = candidates.Where(candidate => analysis.RelatedEntryIds.Contains(candidate.Id)).ToList();

        var opinionRequest = new OpinionRequest(video.Title, channelName, transcript, context.Setup, context.Interests, related, searchResults);
        var opinion = await dependencies.Writer.WriteOpinionAsync(opinionRequest, cancellationToken).ConfigureAwait(false);
        var images = await dependencies.Images.CaptureAsync(video.YouTubeId, analysis.FrameTimes, cancellationToken).ConfigureAwait(false);

        var day = DateOnly.FromDateTime(dependencies.Time.GetLocalNow().DateTime);
        var sources = searchResults.Select(result => new WebSource(result.Title, result.Url)).ToList();
        var relatedIds = related.Select(candidate => candidate.Id).ToList();
        var entry = new NewEntry(video.Id, day, analysis.Headline, analysis.Tags, summary, opinion, sources, images, relatedIds);
        await dependencies.Entries.AddAsync(entry, cancellationToken).ConfigureAwait(false);
    }

    //

    private readonly VideoProcessorDependencies dependencies;

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
}

/// <summary>The services needed to turn a video into an entry.</summary>
public sealed record VideoProcessorDependencies(
    ICaptionSource Captions,
    ISpeechTranscriber Speech,
    IBriefingWriter Writer,
    IWebSearch Search,
    IImageSource Images,
    IEntryStore Entries,
    TimeProvider Time);