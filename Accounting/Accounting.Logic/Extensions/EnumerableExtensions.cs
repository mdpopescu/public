using System.Collections.Generic;
using System.Linq;
using Accounting.Logic.Models;

namespace Accounting.Logic.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Expense> Within(this IEnumerable<Expense> sequence, DateRange dateRange)
        {
            return sequence.Where(it => it.Date >= dateRange.StartDate && it.Date <= dateRange.EndDate);
        }
    }
}