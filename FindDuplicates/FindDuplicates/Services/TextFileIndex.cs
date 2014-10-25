using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FindDuplicates.Contracts;

namespace FindDuplicates.Services
{
  public class TextFileIndex : Index
  {
    public TextFileIndex(string path)
    {
      this.path = path;

      index = ReadIndex();
    }

    public Tuple<long, long> Get(string key)
    {
      Tuple<long, long> result;
      return index.TryGetValue(key, out result) ? result : null;
    }

    public void Set(string key, Tuple<long, long> tuple)
    {
      index[key] = tuple;
      File.AppendAllText(path, string.Format("{0}={1},{2}{3}", key, tuple.Item1, tuple.Item2, Environment.NewLine));
    }

    //

    private readonly string path;

    private readonly Dictionary<string, Tuple<long, long>> index;

    private Dictionary<string, Tuple<long, long>> ReadIndex()
    {
      var lines = File.Exists(path) ? File.ReadAllLines(path) : new string[0];
      return lines
        .Select(GetParts)
        .ToDictionary(parts => parts[0], line => Tuple.Create(long.Parse(line[1]), long.Parse(line[2])));
    }

    private static string[] GetParts(string line)
    {
      var i = line.IndexOf('=');
      var j = line.IndexOf(',', i);

      return new[] { line.Substring(0, i), line.Substring(i + 1, j - i - 1), line.Substring(j + 1) };
    }
  }
}