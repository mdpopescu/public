using System.ComponentModel.DataAnnotations;

namespace Renfield.SafeRedir.Models
{
  public class SummaryInfo
  {
    [Display(Name = "Today")]
    public int Today { get; set; }

    [Display(Name = "Current Month")]
    public int CurrentMonth { get; set; }

    [Display(Name = "Current Year")]
    public int CurrentYear { get; set; }

    [Display(Name = "Overall")]
    public int Overall { get; set; }
  }
}