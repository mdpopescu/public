using System.Collections.Generic;
using System.Linq;

namespace Renfield.AppendOnly.Library
{
  public class GenericAppendOnlyFile<T> : GenericAppendOnly<T>
  {
    public long[] Index
    {
      get { return file.Index; }
    }

    public GenericAppendOnlyFile(LowLevelAppendOnlyFile file, SerializationEngine serializationEngine)
    {
      this.file = file;
      this.serializationEngine = serializationEngine;
    }

    /// <summary>
    /// Adds a record to the file
    /// </summary>
    /// <param name="record">Record to add</param>
    public void Append(T record)
    {
      var bytes = serializationEngine.Serialize(record);
      file.Append(bytes);
    }

    /// <summary>
    /// Returns the i-th record
    /// </summary>
    /// <param name="i">The record number (0-based)</param>
    /// <returns>The contents of the record</returns>
    public T Read(long i)
    {
      var bytes = file.Read(i);

      // the serialization engine might not be thread-safe
      lock (lockObject)
        return serializationEngine.Deserialize<T>(bytes);
    }

    /// <summary>
    /// Returns all records from the i-th on
    /// </summary>
    /// <param name="i">The record number (0-based) to start reading from</param>
    /// <returns>The records from the i-th on</returns>
    public IEnumerable<T> ReadFrom(long i)
    {
      var bytesList = file.ReadFrom(i);

      // the serialization engine might not be thread-safe
      lock (lockObject)
        return bytesList.Select(bytes => serializationEngine.Deserialize<T>(bytes));
    }

    //

    private readonly LowLevelAppendOnlyFile file;
    private readonly SerializationEngine serializationEngine;
    private readonly object lockObject = new object();
  }
}