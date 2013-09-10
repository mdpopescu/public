using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Renfield.AppendOnly.Library;

namespace Renfield.AppendOnly.Spike
{
  internal class Program
  {
    private const int COUNT = 10000;

    private static readonly Random rnd = new Random();

    private static void Main(string[] args)
    {
      var serializer = new ProtoBufSerializationEngine();

      var list = new List<TestClass>();
      for (var i = 0; i < COUNT; i++)
      {
        var c = new TestClass {Name = GenerateRandomString(20), Address = GenerateRandomString(40)};
        list.Add(c);
      }

      var tempFile = Path.GetTempFileName();
      Console.WriteLine("Using {0} - sequentially writing and reading {1} records", tempFile, COUNT);
      Generate(tempFile, serializer, list, SeqForLoop);

      // verify the data
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

      // multi-threaded test
      tempFile = Path.GetTempFileName();
      Console.WriteLine("Using {0} - multi-threaded writing and reading {1} records", tempFile, COUNT);
      Generate(tempFile, serializer, list, ParForLoop);
    }

    private static void Generate(string tempFile, SerializationEngine serializer, List<TestClass> list,
                                 Action<Action<int>> loop)
    {
      using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
      {
        var data = new StreamAccessor(stream);
        var lFile = new LowLevelAppendOnlyFile(data);
        var file = new GenericAppendOnlyFile<TestClass>(lFile, serializer);

        // append COUNT records
        ShowTime("append", () => loop(i => file.Append(list[i])));

        // read all the records
        ShowTime("read all", () => { var records = file.ReadFrom(0).ToList(); });

        // read all the records, one at a time
        ShowTime("read all, one at a time", () => loop(i => file.Read(i)));
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
    }

    private static void SeqForLoop(Action<int> action)
    {
      for (var i = 0; i < COUNT; i++)
        action.Invoke(i);
    }

    private static void ParForLoop(Action<int> action)
    {
      Parallel.For(0, COUNT, action);
    }

    private static void ShowTime(string message, Action action)
    {
      var sw = new Stopwatch();
      sw.Start();

      action.Invoke();

      sw.Stop();
      Console.WriteLine("{0} - time elapsed: {1} msec ({2:0.00} / sec)", message, sw.ElapsedMilliseconds,
        (double) COUNT / sw.ElapsedMilliseconds * 1000.0);
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