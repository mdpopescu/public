using System;
using System.Collections.Generic;
using System.Linq;
using Accounting.Logic;
using Accounting.Logic.Implementations;
using Accounting.Logic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accounting.Tests.Implementations
{
    [TestClass]
    public class AccountingEngineTests
    {
        private List<Expense> list;
        private InMemoryRepository<Expense> repository;

        private AccountingEngine sut;

        [TestInitialize]
        public void SetUp()
        {
            list = new List<Expense>();
            repository = new InMemoryRepository<Expense>(list);

            sut = new AccountingEngine(repository);
        }

        [TestClass]
        public class AddExpense : AccountingEngineTests
        {
            [TestMethod]
            public void AddsTheExpenseToTheRepository()
            {
                SystemSettings.Now = () => new DateTime(2000, 1, 2, 3, 4, 5);

                sut.AddExpense(1m, "x");

                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(new DateTime(2000, 1, 2, 3, 4, 5), list[0].Date);
                Assert.AreEqual(1m, list[0].Amount);
                Assert.AreEqual("x", list[0].Category);
            }
        }

        [TestClass]
        public class GetExpenses : AccountingEngineTests
        {
            [TestMethod]
            public void ReturnsExpensesSummarizedByCategory()
            {
                list.Add(new Expense(1m, "x"));
                list.Add(new Expense(2m, "x"));
                list.Add(new Expense(4m, "y"));

                var result = sut
                    .GetSummary(new DateRange(DateTime.MinValue, DateTime.MaxValue))
                    .OrderBy(it => it.Category)
                    .ToList();

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(3m, result[0].Amount);
                Assert.AreEqual("x", result[0].Category);
                Assert.AreEqual(4m, result[1].Amount);
                Assert.AreEqual("y", result[1].Category);
            }
        }
    }
}