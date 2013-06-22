using System.Collections.Generic;

namespace Renfield.Inventory.Models
{
  public class NewPurchaseOrder
  {
    public string CompanyName { get; set; }
    public List<NewPurchaseOrderItem> Items { get; set; }
  }
}