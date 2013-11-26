using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Data
{
  public class Expense
  {
    public int Id { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
    public decimal Total { get; set; }
    public decimal Covered { get; set; }
  }
}