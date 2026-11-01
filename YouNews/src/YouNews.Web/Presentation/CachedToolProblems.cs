using YouNews.Core.Ports;

namespace YouNews.Web.Presentation;

/// <summary>Remembers the tool check for a while, because it starts processes.</summary>
public sealed class CachedToolProblems : IToolProblems
{
    public static readonly TimeSpan LIFETIME = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan FAILURE_LIFETIME = TimeSpan.FromMinutes(1);

    public CachedToolProblems(IToolHealth health, TimeProvider time)
    {
        this.health = health;
        this.time = time;
    }

    public async Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (cached is null || time.GetUtcNow() >= expiresAt)
                await RefreshAsync(cancellationToken).ConfigureAwait(false);

            return cached!;
        }
        finally
        {
            gate.Release();
        }
    }

    //

    private readonly IToolHealth health;
    private readonly TimeProvider time;
    private readonly SemaphoreSlim gate = new(1, 1);

    private IReadOnlyList<string>? cached;
    private DateTimeOffset expiresAt;

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        try
        {
            cached = await health.FindProblemsAsync(cancellationToken).ConfigureAwait(false);
            expiresAt = time.GetUtcNow() + LIFETIME;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            cached = [$"The tool check itself failed: {exception.Message}"];
            expiresAt = time.GetUtcNow() + FAILURE_LIFETIME;
        }
    }
}