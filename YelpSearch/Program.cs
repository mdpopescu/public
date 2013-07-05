using System;

namespace Renfield.YelpSearch
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var settings = new YelpSettings();

      var yelp = new YelpTool(settings);
      var result = yelp.Search("hotels", "30314", 25);
      Console.WriteLine(result);
    }
  }
}