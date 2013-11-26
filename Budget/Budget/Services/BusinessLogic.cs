using Budget.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Services
{
  public class BusinessLogic : Logic
  {
    public IEnumerable<Expense> GetRecurringExpensesFor(int year, int month)
    {
      return new List<Expense>();
    }

    public IEnumerable<Expense> GetLongTermExpenses()
    {
      return new List<Expense>();
    }
  }
}