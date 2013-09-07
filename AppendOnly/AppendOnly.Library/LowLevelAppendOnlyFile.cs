using System.Collections.Generic;
using System.Linq;

namespace Renfield.AppendOnly.Library
{
  public class LowLevelAppendOnlyFile : LowLevelAppendOnly
  {
    public long[] Index { get; private set; }

    public LowLevelAppendOnlyFile(FileAccessor data, long[] index = null)
    {
      this.data = data;
      Index = index ?? RebuildIndex().ToArray();
    }

    /// <summary>
    /// Adds a record to the file
    /// </summary>
    /// <param name="record">Record to add</param>
    public void Append(byte[] record)
    {
      var position = data.get_length();
      data.write_long(position, record.Length);

      position += sizeof(long);
      data.write_bytes(position, record);
    }

    /// <summary>
    /// Returns the i-th record
    /// </summary>
    /// <param name="i">The record number (0-based)</param>
    /// <returns>The contents of the record</returns>
    public byte[] Read(long i)
    {
      var position = Index[i];

      var size = data.read_long(position);
      position += sizeof (long);

      return data.read_bytes(position, size);
    }

    /// <summary>
    /// Returns all records from the i-th on
    /// </summary>
    /// <param name="i">The record number (0-based) to start reading from</param>
    /// <returns>The records from the i-th on</returns>
    public IEnumerable<byte[]> ReadFrom(long i)
    {
      var length = data.get_length();
      var position = Index[i];
      while (position < length)
      {
        var size = data.read_long(position);
        position += sizeof (long);

        yield return data.read_bytes(position, size);
        position += size;
      }
    }

    //

    private readonly FileAccessor data;

    private IEnumerable<long> RebuildIndex()
    {
      var length = data.get_length();
      long position = 0;
      while (position < length)
      {
        yield return position;

        var size = data.read_long(position);
        position += size + sizeof (long);
      }
    }
  }
}