using System.ComponentModel.DataAnnotations;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Models
{
  public class StockModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Quantity { get; set; }

    [Display(Name = "Recommended Retail Price")]
    public string RRP { get; set; }

    [Display(Name = "Purchase Value")]
    public string PurchaseValue { get; set; }

    [Display(Name = "Sale Value")]
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