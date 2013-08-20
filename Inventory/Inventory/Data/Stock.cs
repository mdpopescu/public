using System.ComponentModel.DataAnnotations;

namespace Renfield.Inventory.Data
{
  public class Stock : Entity
  {
    public int Id { get; set; }
    public int ProductId { get; set; }

    [StringLength(256)]
    public string Name { get; set; }

    public decimal Quantity { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal PurchaseValue { get; set; }
    public decimal SaleValue { get; set; }

    [Timestamp]
    [ScaffoldColumn(false)]
    public byte[] Version { get; set; }

    public virtual Product Product { get; set; }
  }
}