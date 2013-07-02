using System.ComponentModel.DataAnnotations;

namespace Renfield.Inventory.Data
{
  public class Stock
  {
    public int Id { get; set; }
    public int ProductId { get; set; }

    [StringLength(256)]
    public string Name { get; set; }

    public decimal? SalePrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal PurchaseValue { get; set; }
    public decimal SaleValue { get; set; }
  }
}