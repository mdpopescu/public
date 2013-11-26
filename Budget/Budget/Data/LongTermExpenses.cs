using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Budget.Data
{
  public class LongTermExpenses
  {
    public int Id { get; set; }

    public virtual Collection<Expense> Expenses { get; set; }
  }
}