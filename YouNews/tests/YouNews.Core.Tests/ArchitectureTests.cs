using YouNews.Core.Model;

namespace YouNews.Core.Tests;

[TestClass]
public sealed class ArchitectureTests
{
    [TestMethod]
    public void CoreDoesNotReferenceOuterLayersOrFrameworks()
    {
        var allowedPrefixes = new[] { "System", "netstandard", "Microsoft.Extensions.DependencyInjection.Abstractions" };

        var references = typeof(Channel).Assembly.GetReferencedAssemblies().Select(name => name.Name ?? "");

        var offending = references.Where(name => !allowedPrefixes.Any(name.StartsWith)).ToList();
        Assert.IsEmpty(offending, "Core must stay free of frameworks. Offending: " + string.Join(", ", offending));
    }

    [TestMethod]
    public void TranscriptCountsWordsAcrossSegments()
    {
        var sut = new Transcript([new TranscriptSegment(TimeSpan.Zero, "one  two"), new TranscriptSegment(TimeSpan.FromSeconds(2), "three\nfour ")]);

        Assert.AreEqual(4, sut.WordCount);
    }
}