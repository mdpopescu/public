using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class CommandTests
  {
    [TestMethod]
    public void CallsMethodOfTargetObject()
    {
      var obj = new MyClass1();

      obj.SendCommand("M1", 5);

      Assert.AreEqual(5, obj.X);
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }

      public void M1(int x)
      {
        X = x;
      }
    }
  }
}