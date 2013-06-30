namespace Renfield.Inventory.Data
{
  /// <summary>
  ///   This is a simulated view - I will not add it to the InventoryDB class; it's also denormalized
  /// </summary>
  public class Stock
  {
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Quantity { get; set; }
    public decimal PurchaseValue { get; set; }
    public decimal SaleValue { get; set; }

    public virtual Product Product { get; set; }
  }
}