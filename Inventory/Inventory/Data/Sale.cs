using System;
using System.Collections.Generic;

namespace Renfield.Inventory.Data
{
  public class Sale
  {
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime Date { get; set; }

    public virtual ICollection<SaleItem> Items { get; set; }
    public virtual Company Company { get; set; }
  }
}