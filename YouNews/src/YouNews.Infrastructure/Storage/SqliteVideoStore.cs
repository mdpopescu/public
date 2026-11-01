using Dapper;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

public sealed class SqliteVideoStore : IVideoStore
{
    private const string KNOWN_IDS = "SELECT youtube_id FROM videos WHERE channel_id = @ChannelId";

    private const string INSERT = """
        INSERT OR IGNORE INTO videos (channel_id, youtube_id, title, state, last_reason, first_seen_at, first_seen_ticks)
        VALUES (@ChannelId, @YouTubeId, @Title, @State, @Reason, @FirstSeenAt, @FirstSeenTicks)
        """;

    private const string LIST_BY_STATE = """
        SELECT v.id AS Id, v.channel_id AS ChannelId, v.youtube_id AS YouTubeId, v.title AS Title, v.state AS State,
               v.attempts AS Attempts, v.last_reason AS LastReason, v.first_seen_at AS FirstSeenAt
        FROM videos v JOIN channels c ON c.id = v.channel_id
        WHERE v.state = @State AND c.removed = 0
        ORDER BY v.first_seen_ticks DESC, v.id DESC
        """;

    private const string SET_STATE = "UPDATE videos SET state = @State, last_reason = @Reason WHERE id = @VideoId";
    private const string RECORD_FAILURE = "UPDATE videos SET attempts = attempts + 1, last_reason = @Reason WHERE id = @VideoId";

    private readonly ISqliteConnectionFactory factory;

    public SqliteVideoStore(ISqliteConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<IReadOnlySet<string>> GetKnownIdsAsync(long channelId, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
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
        };
        await ExecuteAsync(INSERT, parameters, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TrackedVideo>> ListByStateAsync(VideoState state, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        var rows = await connection.QueryAsync<VideoRow>(new CommandDefinition(LIST_BY_STATE, new { State = state.ToString() }, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return rows.Select(ToTrackedVideo).ToList();
    }

    public Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken) =>
        ExecuteAsync(SET_STATE, new { VideoId = videoId, State = state.ToString(), Reason = reason }, cancellationToken);

    public Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken) =>
        ExecuteAsync(RECORD_FAILURE, new { VideoId = videoId, Reason = reason }, cancellationToken);

    //

    private static TrackedVideo ToTrackedVideo(VideoRow row) => new(
        row.Id,
        row.ChannelId,
        row.YouTubeId,
        row.Title,
        Enum.Parse<VideoState>(row.State),
        row.Attempts,
        row.LastReason,
        StorageFormat.ParseMoment(row.FirstSeenAt));

    private async Task ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken)
    {
        await using var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
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
    }
}