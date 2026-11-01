namespace YouNews.Infrastructure.Processes;

public sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);

/// <summary>Runs external programs.</summary>
public interface IProcessRunner
{
    /// <summary>
    /// Runs a program to completion and captures its output. A non-zero exit code is returned, not thrown.
    /// The program and its children are killed on timeout or cancellation, and a <see cref="TimeoutException"/>
    /// or <see cref="OperationCanceledException"/> is thrown.
    /// </summary>
    /// <param name="fileName">The program to start.</param>
    /// <param name="arguments">The arguments, passed one by one without shell quoting.</param>
    /// <param name="workingDirectory">Null to use the current one.</param>
    /// <param name="timeout">How long the program may run before it is killed.</param>
    /// <param name="cancellationToken">Cancels the run and kills the program.</param>
    Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory, TimeSpan timeout, CancellationToken cancellationToken);
}