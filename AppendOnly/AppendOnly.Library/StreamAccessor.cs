using System;
using System.IO;

namespace Renfield.AppendOnly.Library
{
  public class StreamAccessor : RandomAccessor
  {
    public StreamAccessor(Stream stream)
    {
      this.stream = stream;
    }

    public long get_length()
    {
      lock (lockObject)
        return stream.Length;
    }

    public long read_long(long position)
    {
      var buffer = new byte[sizeof (long)];

      lock (lockObject)
      {
        stream.Seek(position, SeekOrigin.Begin);
        stream.Read(buffer, 0, sizeof (long));
      }

      return BitConverter.ToInt64(buffer, 0);
    }

    public byte[] read_bytes(long position, long size)
    {
      var buffer = new byte[size];

      lock (lockObject)
      {
        stream.Seek(position, SeekOrigin.Begin);
        stream.Read(buffer, 0, (int) (size & 0xFFFFFFFF));
      }

      return buffer;
    }

    public void write_long(long position, long value)
    {
      var buffer = BitConverter.GetBytes(value);

      lock (lockObject)
      {
        stream.Seek(position, SeekOrigin.Begin);
        stream.Write(buffer, 0, sizeof (long));
      }
    }

    public void write_bytes(long position, byte[] value)
    {
      lock (lockObject)
      {
        stream.Seek(position, SeekOrigin.Begin);
        stream.Write(value, 0, value.Length);
      }
    }

    //

    private readonly Stream stream;
    private readonly object lockObject = new object();
  }
}