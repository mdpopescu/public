using System;
using System.Collections.Generic;

namespace Renfield.Inventory.Data
{
  public class Acquisition : Entity
  {
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime Date { get; set; }

    public virtual ICollection<AcquisitionItem> Items { get; set; }
    public virtual Company Company { get; set; }
  }
}