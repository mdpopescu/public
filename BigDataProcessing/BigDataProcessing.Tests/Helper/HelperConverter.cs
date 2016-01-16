using System.Linq;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Tests.Helper
{
  public class HelperConverter : LineConverter
  {
    public string Convert(string line)
    {
      var parts = line
        .Split(',')
        .Select(int.Parse)
        .Select(it => it * 2);

      return string.Join(":", parts);
    }
  }
}