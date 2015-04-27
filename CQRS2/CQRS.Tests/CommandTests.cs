using System;
using System.Threading.Tasks;
using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class CommandTests
  {
    [TestMethod]
    public async Task CallsMethodOfTargetObject()
    {
      var obj = new MyClass1();

      await obj.SendCommandAsync(it => it.M1(5));

      Assert.AreEqual(5, obj.X);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public async Task ExceptionsThrownByTargetMethodGetRethrownAtCallSite()
    {
      var obj = new MyClass1();

      await obj.SendCommandAsync(it => it.M2());
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }

      public void M1(int x)
      {
        X = x;
      }

      public void M2()
      {
        throw new Exception("test");
      }
    }
  }
}