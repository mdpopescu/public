using System.Collections.Generic;

namespace Renfield.Inventory.Data
{
  public class Acquisition
  {
    public int Id { get; set; }
    public int CompanyId { get; set; }

    public virtual ICollection<AcquisitionItem> Items { get; set; }
    public virtual Company Company { get; set; }
  }
}