using System;
using System.Linq;

namespace Renfield.PageFaults
{
  public class LineParser
  {
    public ExecutionContext Parse(string line)
    {
      var items = line.Split(',');
      var algorithm = items[0];
      var size = int.Parse(items[1]);
      var rest = items.Skip(2).Select(int.Parse).ToArray();

      return new ExecutionContext
      {
        Cache = CreateCache(algorithm, size, rest),
        Pages = rest,
      };
    }

    //

    private Cache CreateCache(string algorithm, int size, int[] pages)
    {
      switch (algorithm)
      {
        case "F":
          return new FifoCache(size);

        case "L":
          return new LruCache(size);

        case "O":
          return new OptimalCache(size) { Future = pages };
      }

      throw new ArgumentException(string.Format("Internal error: unknown algorithm [{0}].", algorithm));
    }
  }
}