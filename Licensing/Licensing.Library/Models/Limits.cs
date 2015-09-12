using System;

namespace Renfield.Licensing.Library.Models
{
  [Serializable]
  public class Limits
  {
    public int Days { get; set; }
    public int Runs { get; set; }
  }
}