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
    /// <param name="workingDirectory">Null to use the current one.</param>
    Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory, TimeSpan timeout, CancellationToken cancellationToken);
}