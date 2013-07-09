using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Renfield.Inventory.Data;
using Renfield.Inventory.Helpers;

namespace Renfield.Inventory.Models
{
  public class SaleModel
  {
    [ScaffoldColumn(false)]
    public int Id { get; set; }

    [Display(Name = "Company Name")]
    public string CompanyName { get; set; }

    public string Date { get; set; }

    [Display(Name = "Total Value")]
    [Numeric]
    public string Value { get; set; }

    [ScaffoldColumn(false)]
    public IEnumerable<SaleItemModel> Items { get; set; }

    public static SaleModel From(Sale value)
    {
      return new SaleModel
      {
        Id = value.Id,
        CompanyName = value.Company.Name,
        Date = value.Date.ToString(Constants.DATE_FORMAT),
        Value = value.Items.Select(it => it.Quantity * it.Price).Sum().Formatted(),
        Items = value.Items.Select(SaleItemModel.From).ToList(),
      };
    }
  }
}