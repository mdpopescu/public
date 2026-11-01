using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.Tests.YouTube;

/// <summary>Plays recorded tool output and lets a test create the files a tool would create.</summary>
public sealed class FakeProcessRunner : IProcessRunner
{
    public List<Invocation> Invocations { get; } = [];

    public Func<Invocation, ProcessResult> Behavior { get; set; } = _ => new ProcessResult(0, "", "");

    public Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var invocation = new Invocation(fileName, arguments, timeout);
        Invocations.Add(invocation);
        return Task.FromResult(Behavior(invocation));
    }

    public sealed record Invocation(string FileName, IReadOnlyList<string> Arguments, TimeSpan Timeout)
    {
        /// <summary>Gets the value after the given option, such as the -o template.</summary>
        public string ValueAfter(string option) => Arguments[Arguments.ToList().IndexOf(option) + 1];

        public override string ToString() => $"{FileName} {string.Join(' ', Arguments)}";
    }
}