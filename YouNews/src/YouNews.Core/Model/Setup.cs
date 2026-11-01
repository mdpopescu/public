namespace YouNews.Core.Model;

/// <summary>What the user's AI server looked like at the last successful check.</summary>
/// <param name="Warning">Why the latest check failed, if it did. The other values are then from the last good check.</param>
public sealed record SetupSnapshot(IReadOnlyList<string> Models, string Hardware, DateTimeOffset CheckedAt, string? Warning);