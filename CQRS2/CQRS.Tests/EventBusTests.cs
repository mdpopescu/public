using System;
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

    [TestMethod]
    public void AnExceptionInTheCalledMethodCallsTheExceptionHandler()
    {
      var obj = new MyClass1();

      var called = false;
      UnhandledExceptionEventHandler handler = (sender, e) => { called = true; };

      try
      {
        EventBus.UnhandledException += handler;
        EventBus.Send("E3");

        Thread.Sleep(10);
        Assert.IsTrue(called);
      }
      finally
      {
        EventBus.UnhandledException -= handler;
      }
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }

      public MyClass1()
      {
        EventBus.Subscribe(this, "E1", "E2", "E3");
      }

      public void E1(int x)
      {
        X = x;
      }

      public void E2()
      {
        Thread.Sleep(2000);
      }

      public void E3()
      {
        throw new Exception("test");
      }
    }
  }
}