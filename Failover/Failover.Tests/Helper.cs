using System;
using System.IO;

namespace Renfield.Failover.Tests
{
  public static class Helper
  {
    public static string[] GetLines(this Stream stream)
    {
      stream.Seek(0, SeekOrigin.Begin);
      var reader = new StreamReader(stream);

      return reader
        .ReadToEnd()
        .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    }
  }
}