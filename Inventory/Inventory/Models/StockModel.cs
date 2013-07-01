using Renfield.Inventory.Data;

namespace Renfield.Inventory.Models
{
  public class StockModel
  {
    public string Name { get; set; }
    public string Quantity { get; set; }
    public string PurchaseValue { get; set; }
    public string SaleValue { get; set; }

    public static StockModel From(Stock value)
    {
      return new StockModel
      {
        Name = value.Name,
        Quantity = value.Quantity.Formatted(),
        PurchaseValue = value.PurchaseValue.Formatted(),
        SaleValue = value.SaleValue.Formatted(),
      };
    }
  }
}