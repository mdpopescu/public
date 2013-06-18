using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BinarySearchTimings
{
  internal class Program
  {
    private const int MAX = 100000;
    private const int RUNS = 10000;

    private static void Main(string[] args)
    {
      var list = Enumerable
        .Range(0, MAX)
        .Select(it => it.ToString("000000"))
        .ToList();

      var rng = new Random();
      var randomValues = Enumerable
        .Range(0, RUNS)
        .Select(it => rng.Next(MAX * 2).ToString("000000"))
        .ToList();

      var dict = new Dictionary<string, Func<List<string>, string, int>>
      {
        { "SearchUsingIndexOf", SearchUsingIndexOf },
        { "LinearSearch1", LinearSearch1 },
        { "LinearSearch2", LinearSearch2 },
        { "BinarySearch1", BinarySearch1 },
        { "BinarySearch2", BinarySearch2 },
      };

      var timings = dict
        .Select(it => new { name = it.Key, ticks = Run(i => it.Value(list, randomValues[i]), RUNS) })
        .Select(timing => string.Format("{0,-20} -> {1,12} msec", timing.name, timing.ticks));

      foreach (var timing in timings)
        Console.WriteLine(timing);
    }

    private static long Run(Action<int> action, int count)
    {
      var timer = new Stopwatch();
      timer.Start();
      for (var i = 0; i < count; i++)
        action(i);
      timer.Stop();

      return timer.ElapsedMilliseconds;
    }

    private static int SearchUsingIndexOf(List<string> list, string item)
    {
      return list.IndexOf(item);
    }

    private static int LinearSearch1(List<string> list, string item)
    {
      for (var i = 0; i < list.Count; i++)
        if (list[i] == item)
          return i;

      return -1;
    }

    private static int LinearSearch2(List<string> list, string item)
    {
      list.Add(item);
      var i = 0;
      while (list[i] != item)
        i++;

      list.RemoveAt(list.Count - 1);

      return i == list.Count ? -1 : i;
    }

    private static int BinarySearch1(List<string> list, string item)
    {
      var l = 0;
      var r = list.Count - 1;

      while (l <= r)
      {
        var m = l + (r - l) / 2;
        var comparison = string.CompareOrdinal(list[m], item);

        if (comparison == 0)
          return m;
        else if (comparison < 0)
          l = m + 1;
        else
          r = m - 1;
      }

      return -1;
    }

    private static int BinarySearch2(List<string> list, string item)
    {
      return list.BinarySearch(item);
    }
  }
}