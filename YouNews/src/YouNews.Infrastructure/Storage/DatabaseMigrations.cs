namespace YouNews.Infrastructure.Storage;

/// <summary>The ordered schema scripts. A script's position plus one is its version; never edit one that has shipped.</summary>
public sealed class DatabaseMigrations : IDatabaseMigrations
{
    public IReadOnlyList<string> Scripts { get; } =
    [
        """
        CREATE TABLE channels (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            youtube_id TEXT NOT NULL UNIQUE,
            url TEXT NOT NULL,
            name TEXT NOT NULL,
            removed INTEGER NOT NULL DEFAULT 0
        );
        CREATE TABLE videos (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            channel_id INTEGER NOT NULL REFERENCES channels(id),
            youtube_id TEXT NOT NULL UNIQUE,
            title TEXT NOT NULL,
            state TEXT NOT NULL,
            attempts INTEGER NOT NULL DEFAULT 0,
            last_reason TEXT NULL,
            first_seen_at TEXT NOT NULL,
            first_seen_ticks INTEGER NOT NULL
        );
        CREATE INDEX ix_videos_channel ON videos(channel_id);
        CREATE INDEX ix_videos_state ON videos(state, first_seen_ticks);
        CREATE TABLE entries (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            video_id INTEGER NOT NULL UNIQUE REFERENCES videos(id),
            day TEXT NOT NULL,
            headline TEXT NOT NULL,
            summary TEXT NOT NULL,
            opinion TEXT NOT NULL
        );
        CREATE INDEX ix_entries_day ON entries(day);
        CREATE TABLE entry_tags (
            entry_id INTEGER NOT NULL REFERENCES entries(id),
            position INTEGER NOT NULL,
            tag TEXT NOT NULL,
            PRIMARY KEY (entry_id, position)
        );
        CREATE TABLE entry_web_sources (
            entry_id INTEGER NOT NULL REFERENCES entries(id),
            position INTEGER NOT NULL,
            title TEXT NOT NULL,
            url TEXT NOT NULL,
            PRIMARY KEY (entry_id, position)
        );
        CREATE TABLE entry_images (
            entry_id INTEGER NOT NULL REFERENCES entries(id),
            position INTEGER NOT NULL,
            relative_path TEXT NOT NULL,
            kind TEXT NOT NULL,
            PRIMARY KEY (entry_id, position)
        );
        CREATE TABLE entry_related (
            entry_id INTEGER NOT NULL REFERENCES entries(id),
            related_entry_id INTEGER NOT NULL REFERENCES entries(id),
            position INTEGER NOT NULL,
            PRIMARY KEY (entry_id, related_entry_id)
        );
        CREATE TABLE interests (
            position INTEGER PRIMARY KEY,
            text TEXT NOT NULL
        );
        CREATE TABLE setup_snapshot (
            id INTEGER PRIMARY KEY CHECK (id = 1),
            models TEXT NOT NULL,
            hardware TEXT NOT NULL,
            checked_at TEXT NOT NULL,
            warning TEXT NULL
        );
        CREATE TABLE refresh_runs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            started_at TEXT NOT NULL,
            finished_at TEXT NOT NULL,
            completed INTEGER NOT NULL,
            failed INTEGER NOT NULL,
            problem TEXT NULL
        );
        """,
    ];
}