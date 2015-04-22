using System.Diagnostics;
using System.Threading;
using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class EventBusTests
  {
    [TestMethod]
    public void ObjectsCanSubscribeToEvents()
    {
      var obj = new MyClass1();

      EventBus.Send("E1", 5);

      Thread.Sleep(10);
      Assert.AreEqual(5, obj.X);
    }

    [TestMethod]
    public void TheEventHandlersAreCalledAsynchronously()
    {
      var obj = new MyClass1();

      var sw = new Stopwatch();
      sw.Start();
      EventBus.Send("E2");
      sw.Stop();

      Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }

      public MyClass1()
      {
        EventBus.Subscribe(this, "E1", "E2");
      }

      public void E1(int x)
      {
        X = x;
      }

      public void E2()
      {
        Thread.Sleep(2000);
      }
    }
  }
}