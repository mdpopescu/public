using System;
using System.Collections.Generic;
using System.Linq;
using Accounting.Logic;
using Accounting.Logic.Implementations;
using Accounting.Logic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accounting.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void Works()
        {
            var repository = new InMemoryRepository<Expense>(new List<Expense>());
            var engine = new AccountingEngine(repository);

            SystemSettings.Now = () => new DateTime(2000, 1, 2);
            engine.AddExpense(100m, "groceries");
            engine.AddExpense(50m, "school");

            SystemSettings.Now = () => new DateTime(2000, 1, 3);
            engine.AddExpense(250m, "groceries");

            SystemSettings.Now = () => new DateTime(2000, 1, 4);
            var summary = engine
                .GetSummary(new DateRange(new DateTime(2000, 1, 1), new DateTime(2000, 1, 31)))
                .OrderBy(it => it.Category)
                .ToList();
            Assert.AreEqual(2, summary.Count);
            Assert.AreEqual("groceries", summary[0].Category);
            Assert.AreEqual(350m, summary[0].Amount);
            Assert.AreEqual("school", summary[1].Category);
            Assert.AreEqual(50m, summary[1].Amount);
        }
    }
}