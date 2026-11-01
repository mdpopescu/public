using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>What stays the same for all the videos of one refresh run.</summary>
public sealed record ProcessingContext(IReadOnlyDictionary<long, Channel> Channels, SetupSnapshot? Setup, IReadOnlyList<string> Interests);

/// <summary>Turns one video into a stored entry.</summary>
public interface IVideoProcessor
{
    /// <summary>Gets the transcript, writes the texts, captures images and stores the entry. Throws when the entry cannot be completed.</summary>
    Task ProcessAsync(TrackedVideo video, ProcessingContext context, CancellationToken cancellationToken);
}