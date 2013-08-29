namespace Renfield.Inventory.Data
{
  public class AcquisitionItem
  {
    public int Id { get; set; }
    public int AcquisitionId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual Acquisition Acquisition { get; set; }
    public virtual Product Product { get; set; }
  }
}