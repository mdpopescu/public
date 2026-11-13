namespace YouNews.Core.Model;

/// <summary>Something the user must be told about a run until they dismiss it, such as the run having been stopped.</summary>
public sealed record RunAlert(string Message, DateTimeOffset RaisedAt);