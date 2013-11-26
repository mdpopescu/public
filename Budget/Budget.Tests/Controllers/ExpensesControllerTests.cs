using Budget.Controllers;
using Budget.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Tests.Controllers
{
  [TestClass]
  public class ExpensesControllerTests
  {
    [TestClass]
    public class Index : ExpensesControllerTests
    {
      [TestMethod]
      public void GetsCurrentExpensesFromBusinessLogic()
      {
        var logic = new Mock<Logic>();
        GlobalSettings.SystemTime = () => new DateTime(2000, 1, 2, 3, 4, 5);
        var sut = new ExpensesController(logic.Object);

        sut.Index();

        logic.Verify(it => it.GetRecurringExpensesFor(2000, 1));
      }
    }
  }
}
