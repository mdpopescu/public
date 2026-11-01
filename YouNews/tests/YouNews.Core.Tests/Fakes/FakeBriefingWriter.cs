using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeBriefingWriter : IBriefingWriter
{
    public HashSet<string> FailingTitles { get; } = [];

    public VideoAnalysis Analysis { get; set; } = new("Headline", ["research"], [TimeSpan.FromSeconds(30)], [], []);

    public OpinionRequest? LastOpinionRequest { get; private set; }

    public Transcript? LastTranscript { get; private set; }

    public IReadOnlyList<EntryDigest>? LastCandidates { get; private set; }

    public string Summary { get; set; } = "Summary";

    public string Opinion { get; set; } = "Opinion";

    /// <summary>Everything that happened, in order, as "analyze title" or "opinion title".</summary>
    public List<string> Events { get; } = [];

    /// <summary>Runs while an opinion is being written, so a test can hold the writer.</summary>
    public Func<Task>? DuringOpinion { get; set; }

    public Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken)
    {
        Events.Add("analyze " + title);
        LastTranscript = transcript;
        LastCandidates = candidates;
        return FailingTitles.Contains(title) ? throw new InvalidOperationException("model is down") : Task.FromResult(Analysis);
    }

    public Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken) => Task.FromResult(Summary);

    public async Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken)
    {
        Events.Add("opinion " + request.Title);
        LastOpinionRequest = request;
        if (DuringOpinion is not null)
            await DuringOpinion().ConfigureAwait(false);

        Events.Add("opinion done " + request.Title);
        return Opinion;
    }
}