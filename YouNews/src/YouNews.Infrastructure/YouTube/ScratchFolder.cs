namespace YouNews.Infrastructure.YouTube;

/// <summary>A uniquely named folder below the work directory that is deleted together with the object.</summary>
public sealed class ScratchFolder : IDisposable
{
    public string Path { get; }

    public ScratchFolder(string workDirectory)
    {
        Path = System.IO.Path.Combine(workDirectory, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // Cleanup must not hide the real result; the folder is only scratch space.
        }
    }
}