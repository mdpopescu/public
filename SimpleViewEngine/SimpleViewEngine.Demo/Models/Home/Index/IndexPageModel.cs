using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Demo.Models.Home.Index
{
  public class IndexPageModel
  {
    public string Title { get; set; }
    public Person Boss { get; set; }
    public List<Person> People { get; set; }

    public string LinkToAcquisitions { get; set; }
  }
}