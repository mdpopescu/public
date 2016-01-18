using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Tests.Helper
{
  public class NullConverter : LineConverter
  {
    public string Convert(string line)
    {
      return line;
    }
  }
}