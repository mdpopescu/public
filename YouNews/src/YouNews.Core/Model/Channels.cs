namespace YouNews.Core.Model;

public sealed record ChannelDetails(string YouTubeId, string Url, string Name);

public sealed record Channel(long Id, string Url, string Name);