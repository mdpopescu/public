using System;
using System.IO;

namespace Renfield.AppendOnly.Library
{
  public class StreamAccessor : FileAccessor
  {
    public StreamAccessor(Stream stream)
    {
      this.stream = stream;
    }

    public long get_length()
    {
      return stream.Length;
    }

    public long read_long(long position)
    {
      stream.Seek(position, SeekOrigin.Begin);

      var buffer = new byte[sizeof (long)];
      stream.Read(buffer, 0, sizeof (long));

      return BitConverter.ToInt64(buffer, 0);
    }

    public byte[] read_bytes(long position, long size)
    {
      stream.Seek(position, SeekOrigin.Begin);

      var buffer = new byte[size];
      stream.Read(buffer, 0, (int) (size & 0xFFFFFFFF));

      return buffer;
    }

    public void write_long(long position, long value)
    {
      stream.Seek(position, SeekOrigin.Begin);

      var buffer = BitConverter.GetBytes(value);
      stream.Write(buffer, 0, sizeof (long));
    }

    public void write_bytes(long position, byte[] value)
    {
      stream.Seek(position, SeekOrigin.Begin);
      stream.Write(value, 0, value.Length);
    }

    //

    private readonly Stream stream;
  }
}