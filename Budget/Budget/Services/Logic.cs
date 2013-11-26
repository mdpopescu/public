using Budget.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Services
{
  public interface Logic
  {
    IEnumerable<Expense> GetRecurringExpensesFor(int year, int month);
    IEnumerable<Expense> GetLongTermExpenses();
  }
}