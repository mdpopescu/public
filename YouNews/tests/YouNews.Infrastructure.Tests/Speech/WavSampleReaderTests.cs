using YouNews.Infrastructure.Speech;

namespace YouNews.Infrastructure.Tests.Speech;

[TestClass]
public sealed class WavSampleReaderTests
{
    [TestInitialize]
    public void Setup()
    {
        folder = Path.Combine(AppContext.BaseDirectory, "wav-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(folder, true);

    [TestMethod]
    public void ReadsSamplesInBlocksAndScalesThemToFloats()
    {
        var path = WriteWav(16000, [0, 16384, -32768, 32767, 100]);

        using var sut = new WavSampleReader(path);
        var first = sut.Read(3);
        var second = sut.Read(3);
        var third = sut.Read(3);

        CollectionAssert.AreEqual(new[] { 0f, 0.5f, -1f }, first);
        Assert.HasCount(2, second);
        Assert.AreEqual(32767 / 32768f, second[0]);
        Assert.IsEmpty(third, "the end of the file yields an empty block");
    }

    [TestMethod]
    public void SkipsExtraChunksBeforeTheData()
    {
        var path = WriteWav(16000, [1000], true);

        using var sut = new WavSampleReader(path);

        Assert.AreEqual(1000 / 32768f, sut.Read(10).Single());
    }

    [TestMethod]
    public void RejectsAnotherSampleRate()
    {
        var path = WriteWav(44100, [1, 2, 3]);

        var exception = Assert.Throws<InvalidDataException>(() => new WavSampleReader(path));

        Assert.Contains("44100", exception.Message);
    }

    //

    private string folder = "";

    private string WriteWav(int sampleRate, short[] samples, bool withListChunk = false)
    {
        var path = Path.Combine(folder, Guid.NewGuid().ToString("N") + ".wav");
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream);
        writer.Write("RIFF"u8);
        writer.Write(36 + samples.Length * 2);
        writer.Write("WAVE"u8);
        writer.Write("fmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(sampleRate);
        writer.Write(sampleRate * 2);
        writer.Write((short)2);
        writer.Write((short)16);
        if (withListChunk)
        {
            writer.Write("LIST"u8);
            writer.Write(5);
            writer.Write("abcde"u8);
            writer.Write((byte)0);
        }
        writer.Write("data"u8);
        writer.Write(samples.Length * 2);
        foreach (var sample in samples)
            writer.Write(sample);
        return path;
    }
}