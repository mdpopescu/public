using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Renfield.Sudoku.Solver
{
  // from http://norvig.com/sudoku.html
  internal static class Program
  {
    private const string digits = "123456789";

    private const string rows = "ABCDEFGHI";
    private const string cols = digits;

    private static List<string> squares;
    private static Dictionary<string, List<List<string>>> units;
    private static Dictionary<string, List<string>> peers;

    private static void Main(string[] args)
    {
      var unitlist = (from c in cols select Cross(rows, c))
        .Concat(from r in rows select Cross(r, cols))
        .Concat(from rs in new[] {"ABC", "DEF", "GHI"} from cs in new[] {"123", "456", "789"} select Cross(rs, cs))
        .ToList();

      squares = Cross(rows, cols).ToList();
      units = squares.ToDictionary(s => s, s => unitlist.Where(u => u.Contains(s)).ToList());
      peers = squares.ToDictionary(s => s, s => units[s].Flatten().Except(new[] {s}).ToList());

      Tests(unitlist);

      var puzzles = new[]
      {
        "003020600900305001001806400008102900700000008006708200002609500800203009005010300",
        "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......",
        "85...24..72......9..4.........1.7..23.5...9...4...........8..7..17..........36.4.",
        "..53.....8......2..7..1.5..4....53...1..7...6..32...8..6.5....9..4....3......97..",
        ".....6....59.....82....8....45........3........6..3.54...325..6..................", // hard
      };

      var index = 0;
      foreach (var puzzle in puzzles)
        Display(string.Format("[{0}]", index++), Solve(puzzle));
    }

    private static void Tests(List<List<string>> unitlist)
    {
      Debug.Assert(squares.Count == 81);
      Debug.Assert(unitlist.Count == 27);
      Debug.Assert(squares.All(s => units[s].Count == 3));
      Debug.Assert(squares.All(s => peers[s].Count == 20));
      Debug.Assert(units["C2"].IsEqualTo(new List<List<string>>
      {
        new List<string> {"A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2", "I2"},
        new List<string> {"C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9"},
        new List<string> {"A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3"},
      }));
      Debug.Assert(peers["C2"].IsEqualTo(new List<string>
      {
        "A2",
        "B2",
        "D2",
        "E2",
        "F2",
        "G2",
        "H2",
        "I2",
        "C1",
        "C3",
        "C4",
        "C5",
        "C6",
        "C7",
        "C8",
        "C9",
        "A1",
        "A3",
        "B1",
        "B3",
      }));

      Console.WriteLine("All tests pass.");
    }

    private static void Display(string name, IReadOnlyDictionary<string, string> values)
    {
      Console.WriteLine(name);

      var width = 1 + squares.Select(s => values[s].Length).Max();
      var line = string.Join("+", new string('-', width * 3).Times(3));

      foreach (var r in rows)
      {
        Console.WriteLine(string.Join("",
          cols.Select(c => values[r.ToString() + c.ToString()].Center(width) +
                           (c.In('3', '6') ? "|" : ""))));
        if (r.In('C', 'F'))
          Console.WriteLine(line);
      }
    }

    private static IEnumerable<string> Times(this string s, int count)
    {
      return Enumerable
        .Range(1, count)
        .Select(_ => s);
    }

    private static string Center(this string s, int width)
    {
      var result = new List<string>(s.Explode());
      while (result.Count < width)
      {
        if (result.Count % 2 == 0)
          result.Add(" ");
        else
          result.Insert(0, " ");
      }

      return string.Join("", result);
    }

    private static Dictionary<string, string> Solve(string grid)
    {
      var sw = new Stopwatch();
      sw.Start();

      var result = Search(ParseGrid(grid));

      sw.Stop();
      Console.WriteLine("Elapsed time: {0} msec.", sw.ElapsedMilliseconds);

      return result;
    }

    private static Dictionary<string, string> Search(Dictionary<string, string> values)
    {
      if (values == null)
        return null; // failed earlier
      if (squares.All(s => values[s].Length == 1))
        return values; // solved

      var min = squares
        .Select(s => new {s, len = values[s].Length})
        .Where(sl => sl.len > 1)
        .OrderBy(sl => sl.len)
        .First()
        .s;
      var seq = values[min]
        .Select(d => Search(Assign(values.Copy(), min, d)));

      return seq.Where(it => it != null).FirstOrDefault();
    }

    private static Dictionary<string, string> Copy(this Dictionary<string, string> dict)
    {
      return new Dictionary<string, string>(dict);
    }

    private static Dictionary<string, string> ParseGrid(string grid)
    {
      var values = squares.ToDictionary(s => s, s => digits);

      return GridValues(grid)
               .Any(sd => sd.Value.In(digits) && Assign(values, sd.Key, sd.Value) == null)
               ? null
               : values;
    }

    private static Dictionary<string, string> Assign(Dictionary<string, string> values, string square, char d)
    {
      var otherValues = values[square].Replace(d.ToString(), "");
      return otherValues.All(d2 => Eliminate(values, square, d2) != null) ? values : null;
    }

    private static Dictionary<string, string> Eliminate(Dictionary<string, string> values, string square, char d)
    {
      if (!d.In(values[square]))
        return values;

      values[square] = values[square].Replace(d.ToString(), "");
      if (values[square].Length == 0)
        return null; // removed last value

      if (values[square].Length == 1)
      {
        var d2 = values[square][0];
        if (peers[square].Any(s2 => Eliminate(values, s2, d2) == null))
          return null;
      }

      var possibles = units[square]
        .Select(u => u.Where(ss => d.In(values[ss])).ToList())
        .ToList();
      foreach (var dplaces in possibles)
      {
        if (dplaces.Count == 0)
          return null; // no place for this value

        if (dplaces.Count == 1 && Assign(values, dplaces[0], d) == null)
          return null;
      }

      return values;
    }

    private static Dictionary<string, char> GridValues(string grid)
    {
      var chars = grid.Where(c => char.IsDigit(c) || c == '0' || c == '.').ToList();
      Debug.Assert(chars.Count == 81);

      return squares.Zip(chars).ToDictionary();
    }

    private static List<string> Cross(object A, object B)
    {
      return (from a in A.ToString().Explode()
              from b in B.ToString().Explode()
              select a + b).ToList();
    }

    private static IEnumerable<string> Explode(this string s)
    {
      return s.Select(c => c.ToString()).ToList();
    }

    private static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> list)
    {
      return list.SelectMany(it => it);
    }

    private static bool IsEqualTo(this IEnumerable<List<string>> expected, IEnumerable<List<string>> actual)
    {
      return expected
        .Zip(actual)
        .All(vv => vv.Item1.IsEqualTo(vv.Item2));
    }

    private static bool IsEqualTo(this IEnumerable<string> expected, IEnumerable<string> actual)
    {
      return expected
        .Zip(actual)
        .All(vv => vv.Item1 == vv.Item2);
    }

    private static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> list1, IEnumerable<T2> list2)
    {
      return list1.Zip(list2, (v1, v2) => new Tuple<T1, T2>(v1, v2));
    }

    private static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<Tuple<T1, T2>> list)
    {
      return list.ToDictionary(it => it.Item1, it => it.Item2);
    }

    private static bool In<T>(this T value, IEnumerable<T> list)
    {
      return list.Contains(value);
    }

    private static bool In<T>(this T value, params T[] list)
    {
      return value.In(list.AsEnumerable());
    }
  }
}