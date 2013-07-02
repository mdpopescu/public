﻿using System.ComponentModel.DataAnnotations;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Models
{
  public class AcquisitionItemModel
  {
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    [Required]
    public string ProductName { get; set; }

    [Required]
    public string Quantity { get; set; }

    [Required]
    public string Price { get; set; }

    public string Value { get; set; }

    public static AcquisitionItemModel From(AcquisitionItem value)
    {
      return new AcquisitionItemModel
      {
        Id = value.Id,
        ProductName = value.Product.Name,
        Quantity = value.Quantity.Formatted(),
        Price = value.Price.Formatted(),
        Value = (value.Quantity * value.Price).Formatted(),
      };
    }

    public bool IsValid()
    {
      return !ProductName.IsNullOrEmpty() &&
             !Quantity.IsNullOrEmpty() &&
             !Price.IsNullOrEmpty();
    }
  }
}