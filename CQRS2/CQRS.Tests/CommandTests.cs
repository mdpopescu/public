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