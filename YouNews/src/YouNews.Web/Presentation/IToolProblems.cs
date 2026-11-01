namespace YouNews.Web.Presentation;

/// <summary>Tells the problems with the external tools, without ever failing.</summary>
public interface IToolProblems
{
    /// <summary>Gets the problems; a failed check is itself reported as a problem.</summary>
    Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken);
}