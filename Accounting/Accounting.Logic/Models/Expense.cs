using System;

namespace Accounting.Logic.Models
{
    public class Expense
    {
        public DateTime Date { get; private set; }
        public decimal Amount { get; private set; }
        public string Category { get; private set; }

        public Expense(decimal amount, string category)
        {
            Date = SystemSettings.Now();
            Amount = amount;
            Category = category;
        }
    }
}