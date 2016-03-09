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
      // The maximum index in any single dimension is 2,147,483,591 (0x7FFFFFC7) for byte arrays
      // https://msdn.microsoft.com/en-us/library/hh285054(v=vs.110).aspx
      var iSize = (int) (size & 0xFFFFFFFF);

      var buffer = new byte[iSize];

      lock (lockObject)
      {
        stream.Seek(position, SeekOrigin.Begin);
        stream.Read(buffer, 0, iSize);
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

    private readonly object lockObject = new object();

    private readonly Stream stream;
  }
}