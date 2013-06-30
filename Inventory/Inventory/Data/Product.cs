using System.ComponentModel.DataAnnotations;

namespace Renfield.Inventory.Data
{
  public class Product
  {
    public int Id { get; set; }
    [StringLength(256)]
    public string Name { get; set; }
    public decimal SalePrice { get; set; }
  }
}