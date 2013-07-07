using System.ComponentModel.DataAnnotations;
using Renfield.Inventory.Data;
using Renfield.Inventory.Helpers;

namespace Renfield.Inventory.Models
{
  public class StockModel
  {
    [ScaffoldColumn(false)]
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string Name { get; set; }

    [Display(Name = "Quantity")]
    [Numeric]
    public string Quantity { get; set; }

    [Display(Name = "Recommended Retail Price")]
    [Numeric]
    public string RRP { get; set; }

    [Display(Name = "Purchase Value")]
    [Numeric]
    public string PurchaseValue { get; set; }

    [Display(Name = "Sale Value")]
    [Numeric]
    public string SaleValue { get; set; }

    public static StockModel From(Stock value)
    {
      return new StockModel
      {
        Id = value.Id,
        Name = value.Name,
        Quantity = value.Quantity.Formatted(),
        RRP = value.SalePrice.Formatted(),
        PurchaseValue = value.PurchaseValue.Formatted(),
        SaleValue = value.SaleValue.Formatted(),
      };
    }
  }
}