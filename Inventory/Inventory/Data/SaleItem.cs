namespace Renfield.Inventory.Data
{
  public class SaleItem : Entity, Item
  {
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual Sale Sale { get; set; }
    public virtual Product Product { get; set; }
  }
}