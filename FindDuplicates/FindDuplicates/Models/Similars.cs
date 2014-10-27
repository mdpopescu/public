using System.Collections.Generic;

namespace FindDuplicates.Models
{
  public class Similars
  {
    public string FileName { get; private set; }
    public List<Similar> List { get; private set; }

    public Similars(string fileName)
    {
      FileName = fileName;
      List = new List<Similar>();
    }
  }
}