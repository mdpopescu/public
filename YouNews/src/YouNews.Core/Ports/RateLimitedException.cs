namespace YouNews.Core.Ports;

/// <summary>YouTube refused a request because too many were made. Further requests are likely to fail the same way for a while.</summary>
public sealed class RateLimitedException(string message) : Exception(message);