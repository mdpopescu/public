using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteVideoStore : IVideoStore
{
    public SqliteVideoStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<IReadOnlySet<string>> GetKnownIdsAsync(long channelId, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var ids = await connection.QueryAsync<string>(new CommandDefinition(KNOWN_IDS, new { ChannelId = channelId }, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return ids.ToHashSet();
    }

    public async Task AddAsync(NewVideo video, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            video.ChannelId,
            video.YouTubeId,
            video.Title,
            State = video.State.ToString(),
            video.Reason,
            FirstSeenAt = StorageFormat.ToText(video.FirstSeenAt),
            FirstSeenTicks = video.FirstSeenAt.UtcTicks,
            PublishedOn = video.PublishedOn is { } publishedOn ? StorageFormat.ToText(publishedOn) : null,
        };
        await ExecuteAsync(INSERT, parameters, cancellationToken).ConfigureAwait(false);
    }

    public Task SetPublishedOnIfMissingAsync(string youTubeId, DateOnly publishedOn, CancellationToken cancellationToken) =>
        ExecuteAsync(SET_PUBLISHED_ON, new { YouTubeId = youTubeId, PublishedOn = StorageFormat.ToText(publishedOn) }, cancellationToken);

    public async Task<IReadOnlyList<TrackedVideo>> ListByStateAsync(VideoState state, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var rows = await connection.QueryAsync<VideoRow>(new CommandDefinition(LIST_BY_STATE, new { State = state.ToString() }, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return rows.Select(ToTrackedVideo).ToList();
    }

    public Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken) =>
        ExecuteAsync(SET_STATE, new { VideoId = videoId, State = state.ToString(), Reason = reason }, cancellationToken);

    public Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken) =>
        ExecuteAsync(RECORD_FAILURE, new { VideoId = videoId, Reason = reason }, cancellationToken);

    //

    private const string KNOWN_IDS = "SELECT youtube_id FROM videos WHERE channel_id = @ChannelId";

    private const string INSERT = """
        INSERT OR IGNORE INTO videos (channel_id, youtube_id, title, state, last_reason, first_seen_at, first_seen_ticks, published_on)
        VALUES (@ChannelId, @YouTubeId, @Title, @State, @Reason, @FirstSeenAt, @FirstSeenTicks, @PublishedOn)
        """;

    private const string LIST_BY_STATE = """
        SELECT v.id AS Id, v.channel_id AS ChannelId, v.youtube_id AS YouTubeId, v.title AS Title, v.state AS State,
               v.attempts AS Attempts, v.last_reason AS LastReason, v.first_seen_at AS FirstSeenAt, v.published_on AS PublishedOn
        FROM videos v JOIN channels c ON c.id = v.channel_id
        WHERE v.state = @State AND c.removed = 0
        ORDER BY date(v.first_seen_at, 'localtime') DESC, v.published_on DESC, v.first_seen_ticks DESC, v.id DESC
        """;

    private const string SET_PUBLISHED_ON = "UPDATE videos SET published_on = @PublishedOn WHERE youtube_id = @YouTubeId AND published_on IS NULL";
    private const string SET_STATE = "UPDATE videos SET state = @State, last_reason = @Reason WHERE id = @VideoId";
    private const string RECORD_FAILURE = "UPDATE videos SET attempts = attempts + 1, last_reason = @Reason WHERE id = @VideoId";

    private readonly ISqliteConnectionFactory factory;

    private static TrackedVideo ToTrackedVideo(VideoRow row) => new(
        row.Id,
        row.ChannelId,
        row.YouTubeId,
        row.Title,
        Enum.Parse<VideoState>(row.State),
        row.Attempts,
        row.LastReason,
        StorageFormat.ParseMoment(row.FirstSeenAt),
        row.PublishedOn is { } publishedOn ? StorageFormat.ParseDay(publishedOn) : null);

    private async Task ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    private sealed class VideoRow
    {
        public long Id { get; set; }

        public long ChannelId { get; set; }

        public string YouTubeId { get; set; } = "";

        public string Title { get; set; } = "";

        public string State { get; set; } = "";

        public int Attempts { get; set; }

        public string? LastReason { get; set; }

        public string FirstSeenAt { get; set; } = "";

        public string? PublishedOn { get; set; }
    }
}