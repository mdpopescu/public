using System;
using System.Collections.Generic;
using System.IO;
using TextReader = BigDataProcessing.Library.Contracts.TextReader;

namespace BigDataProcessing.Library.Services
{
  public class TextStreamReader : TextReader
  {
    public IEnumerable<string> Read(object source)
    {
      var stream = source as Stream;
      if (stream == null)
        throw new ArgumentException("Invalid argument (expected stream)", nameof(source));

      var reader = new StreamReader(stream);

      string line;
      while ((line = reader.ReadLine()) != null)
        yield return line;

      // Do NOT dispose the reader; it will be eventually finalized and NOT dispose the underlying stream
    }
  }
}