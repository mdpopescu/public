using System;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Models
{
  public class AcquisitionModel
  {
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public DateTime Date { get; set; }

    public static AcquisitionModel From(Acquisition value)
    {
      return new AcquisitionModel
      {
        Id = value.Id,
        CompanyName = value.Company.Name,
        Date = value.Date.Date,
      };
    }
  }
}