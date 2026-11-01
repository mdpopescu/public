using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeImageSource : IImageSource
{
    public int Captures { get; private set; }

    public IReadOnlyList<EntryImage> Images { get; set; } = [new("id/thumbnail.jpg", ImageKind.Thumbnail)];

    public Task<IReadOnlyList<EntryImage>> CaptureAsync(string youTubeId, IReadOnlyList<TimeSpan> frameTimes, CancellationToken cancellationToken)
    {
        Captures++;
        return Task.FromResult(Images);
    }
}