using System.ComponentModel.DataAnnotations;

namespace Renfield.Inventory.Data
{
  /// <summary>
  ///   This is a simulated view - I will not add it to the InventoryDB class; it's also denormalized
  /// </summary>
  public class Stock
  {
    public int Id { get; set; }
    public int ProductId { get; set; }

    [StringLength(256)]
    public string Name { get; set; }

    [DisplayFormat(DataFormatString = "#,##0.00")]
    public decimal Quantity { get; set; }

    [DisplayFormat(DataFormatString = "#,##0.00")]
    public decimal PurchaseValue { get; set; }

    [DisplayFormat(DataFormatString = "#,##0.00")]
    public decimal SaleValue { get; set; }
  }
}