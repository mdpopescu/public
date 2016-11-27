using System.Collections.Generic;
using System.Linq;
using Accounting.Logic.Contracts;
using Accounting.Logic.Extensions;
using Accounting.Logic.Models;

namespace Accounting.Logic.Implementations
{
    public class AccountingEngine
    {
        public AccountingEngine(Repository<Expense> repository)
        {
            this.repository = repository;
        }

        public void AddExpense(decimal amount, string category)
        {
            repository.Add(new Expense(amount, category));
        }

        public IEnumerable<Expense> GetSummary(DateRange dateRange)
        {
            return repository
                .GetAll()
                .Within(dateRange)
                .GroupBy(it => it.Category)
                .Select(g => new Expense(g.Sum(it => it.Amount), g.Key));
        }

        //

        private readonly Repository<Expense> repository;
    }
}