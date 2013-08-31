using System.ComponentModel.DataAnnotations;

namespace Renfield.SimpleViewEngine.Demo.Models.Home.Acquisitions
{
  public class AcquisitionItem
  {
    [ScaffoldColumn(false)]
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string ProductName { get; set; }

    public string Quantity { get; set; }
    public string Price { get; set; }
    public string Value { get; set; }
  }
}