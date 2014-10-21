using System;
using FindDuplicates.Contracts;

namespace FindDuplicates.Services
{
  public class ConsoleLogger : Logger
  {
    public void WriteLine(string s)
    {
      Console.WriteLine("{0:hh:mm:ss} {1}", DateTime.Now, s);
    }
  }
}