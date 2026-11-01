using System.Text;

namespace YouNews.Infrastructure.Speech;

/// <summary>Streams the samples of a 16 kHz mono 16-bit PCM WAV file as floats, so a long file is never held in memory.</summary>
public sealed class WavSampleReader : IDisposable
{
    public const int SAMPLE_RATE = 16000;

    public WavSampleReader(string path)
    {
        stream = File.OpenRead(path);
        try
        {
            remainingBytes = FindSamples();
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    /// <summary>Reads up to the given number of samples. Returns an empty array at the end of the file.</summary>
    public float[] Read(int count)
    {
        var wanted = (int)Math.Min((long)count * 2, remainingBytes);
        var buffer = new byte[wanted];
        var received = stream.ReadAtLeast(buffer, wanted, false);
        remainingBytes -= received;

        var samples = new float[received / 2];
        for (var i = 0; i < samples.Length; i++)
            samples[i] = BitConverter.ToInt16(buffer, i * 2) / 32768f;
        return samples;
    }

    public void Dispose() => stream.Dispose();

    //

    private readonly FileStream stream;
    private long remainingBytes;

    private static void CheckFormat(BinaryReader reader, long size)
    {
        var format = reader.ReadUInt16();
        var channels = reader.ReadUInt16();
        var sampleRate = reader.ReadUInt32();
        reader.ReadBytes(6);
        var bitsPerSample = reader.ReadUInt16();
        reader.ReadBytes((int)(size - 16 + (size & 1)));

        if (format != 1 || channels != 1 || sampleRate != SAMPLE_RATE || bitsPerSample != 16)
            throw new InvalidDataException($"Expected 16 kHz mono 16-bit PCM but found format {format}, {channels} channels, {sampleRate} Hz, {bitsPerSample} bits.");
    }

    private static string ReadTag(BinaryReader reader) => Encoding.ASCII.GetString(reader.ReadBytes(4));

    private long FindSamples()
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, true);
        if (ReadTag(reader) != "RIFF")
            throw new InvalidDataException("Not a WAV file.");
        reader.ReadUInt32();
        if (ReadTag(reader) != "WAVE")
            throw new InvalidDataException("Not a WAV file.");

        while (stream.Position + 8 <= stream.Length)
        {
            var tag = ReadTag(reader);
            long size = reader.ReadUInt32();
            if (tag == "data")
                return Math.Min(size, stream.Length - stream.Position);
            if (tag == "fmt ")
                CheckFormat(reader, size);
            else
                stream.Seek(size + (size & 1), SeekOrigin.Current);
        }

        throw new InvalidDataException("The WAV file has no sample data.");
    }
}