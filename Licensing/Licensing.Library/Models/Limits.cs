using System;

namespace Renfield.Licensing.Library.Models
{
  [Serializable]
  public class Limits
  {
    public int Days { get; set; }
    public int Runs { get; set; }

    public int GetRemainingDays(DateTime createdOn)
    {
      if (Days < 0)
        return int.MaxValue;

      var elapsed = (int) Math.Round(DateTime.Today.Subtract(createdOn).TotalDays);
      var remaining = Days - elapsed + 1; // expires at the end of the day, not the beginning

      return remaining <= 0 ? 0 : remaining;
    }
  }
}