﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Data
{
  public interface Persistence
  {
    IEnumerable<Expense> GetExpenses();
  }
}