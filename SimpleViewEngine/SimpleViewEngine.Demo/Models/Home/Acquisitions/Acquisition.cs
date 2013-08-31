using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Renfield.SimpleViewEngine.Demo.Models.Home.Acquisitions
{
  public class Acquisition
  {
    public int Id { get; set; }

    [Display(Name = "Company Name")]
    public string CompanyName { get; set; }

    public string Date { get; set; }

    [Display(Name = "Total Value")]
    public string Value { get; set; }

    [ScaffoldColumn(false)]
    public IEnumerable<AcquisitionItem> Items { get; set; }
  }
}