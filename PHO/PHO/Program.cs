using System;
using System.Linq;
using System.Threading;
using PHO.Models;

namespace PHO
{
    internal class Program
    {
        private static void Main()
        {
            //create book from https://parahumans.wordpress.com/table-of-contents/
            //  create chapter from each(//div.entry-content p strong a).first(div.entry-content)

            var start = new Link("https://parahumans.wordpress.com/table-of-contents/").Load();
            var chaptersQuery = from a in start.Each("//div[@class='entry-content']/p/strong/a")
                                let link = a.ToLink()
                                let title = a.Text.Trim()
                                where !string.IsNullOrWhiteSpace(title)
                                let _ = WithDelay(TimeSpan.FromSeconds(2), $"Loading {title}")
                                let page = link.Load()
                                let contents = string.Concat(Environment.NewLine, page.Each("//div[@class='entry-content']"))
                                select contents;
            var chapters = chaptersQuery.ToList();

            //var chapters = new Link("https://parahumans.wordpress.com/table-of-contents/")
            //    .Load()
            //    .Each("//div[@class='entry-content']/p/strong/a")
            //    .Select(it => it.ToLink())
            //    .WithDelay(TimeSpan.FromSeconds(2))
            //    .Select(link => link.Load())
            //    .Select(page => page.Each("//div[@class='entry-content']").First())
            //    .ToList();
        }

        private static object WithDelay(TimeSpan duration, string text)
        {
            Thread.Sleep(duration);
            Console.WriteLine(text);
            return null;
        }
    }
}