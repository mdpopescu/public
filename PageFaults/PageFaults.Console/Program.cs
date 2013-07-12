using System.IO;
using System.Linq;

namespace Renfield.PageFaults.Console
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      if (args.Length < 1)
      {
        System.Console.WriteLine("Usage: PageFaults.Console <file name>");
        System.Console.WriteLine("       where <file name> contains a number of lines of the form:");
        System.Console.WriteLine("         F,3,0,1,2,3,...");
        System.Console.WriteLine("       The first item should be the page replacement algorithm");
        System.Console.WriteLine("         with F = FIFO, L = LRU, O = Optimal.");
        System.Console.WriteLine("       The second item should be the cache size (3 in this case).");
        System.Console.WriteLine("       The rest of the items represent the pages being accessed.");
        System.Console.WriteLine("       The program will display the pages in the cache for each step and end with the total number of page faults.");

        return;
      }

      var parser = new LineParser();
      var lines = File.ReadAllLines(args[0]);
      foreach (var line in lines)
      {
        System.Console.WriteLine(line);
        RunOnce(parser.Parse(line));
      }
    }

    private static void RunOnce(ExecutionContext context)
    {
      foreach (var page in context.Pages)
      {
        context.Cache.AddPage(page);
        System.Console.WriteLine(page + ": " + string.Join(",", context.Cache.Pages.Select(it => it.HasValue ? it.Value.ToString() : " ")));
      }

      System.Console.WriteLine("Page faults: " + context.Cache.PageFaults);
      System.Console.WriteLine();
    }
  }
}