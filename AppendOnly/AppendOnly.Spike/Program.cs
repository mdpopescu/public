using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Renfield.AppendOnly.Library;

namespace Renfield.AppendOnly.Spike
{
  internal class Program
  {
    private const int COUNT = 100000;

    private static readonly Random rnd = new Random();

    private static void Main(string[] args)
    {
      var serializer = new ProtoBufSerializationEngine();

      var tempFile = Path.GetTempFileName();
      Console.WriteLine("Using {0} - writing and reading {1} records", tempFile, COUNT);

      var list = new List<TestClass>();

      using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
      {
        var data = new StreamAccessor(stream);
        var lFile = new LowLevelAppendOnlyFile(data);
        var file = new GenericAppendOnlyFile<TestClass>(lFile, serializer);

        // append COUNT records
        ShowTime("append", () =>
        {
          for (var i = 0; i < COUNT; i++)
          {
            var c = new TestClass {Name = GenerateRandomString(20), Address = GenerateRandomString(40)};
            list.Add(c);
            file.Append(c);
          }
        });

        // read all the records
        ShowTime("read all", () =>
        {
          var records = file.ReadFrom(0).ToList();
        });
      }

      // close and reopen the file (rebuilds the index)
      ShowTime("rebuild index", () =>
      {
        using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
          var data = new StreamAccessor(stream);
          var lFile = new LowLevelAppendOnlyFile(data);
        }
      });

      using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
      {
        var data = new StreamAccessor(stream);
        var lFile = new LowLevelAppendOnlyFile(data);
        var file = new GenericAppendOnlyFile<TestClass>(lFile, serializer);

        var records = file.ReadFrom(0).ToList();

        // verify that the index is built correctly
        for (var i = 0; i < COUNT; i++)
        {
          var r = records[i];
          Debug.Assert(r.Name == list[i].Name);
          Debug.Assert(r.Address == list[i].Address);
        }
      }
    }

    private static void ShowTime(string message, Action action)
    {
      var sw = new Stopwatch();
      sw.Start();

      action.Invoke();

      sw.Stop();
      Console.WriteLine("{0} - time elapsed: {1} msec", message, sw.ElapsedMilliseconds);
    }

    private static string GenerateRandomString(int maxLength)
    {
      var length = rnd.Next(maxLength / 2, maxLength + 1);

      var result = new List<char>();
      for (var i = 0; i < length; i++)
        result.Add((char) (rnd.Next('A', 'Z' + 1)));

      return new string(result.ToArray());
    }
  }
}