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
    public void ObjectsCanUnsubscribeFromEvents()
    {
      var obj = new MyClass1();

      EventBus.Send("E4");
      EventBus.Send("NoE4");
      Thread.Sleep(10); // give it time for the de-registration to execute
      EventBus.Send("E4");

      Thread.Sleep(10);
      Assert.AreEqual(1, obj.E4Calls);
    }

    [TestMethod]
    public void MethodsAreInvokedAsynchronously()
    {
      var obj = new MyClass1();

      var sw = new Stopwatch();
      sw.Start();
      EventBus.Send("E2");
      sw.Stop();

      Thread.Sleep(10);
      Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }
      public int E4Calls { get; private set; }

      public MyClass1()
      {
        EventBus.Subscribe(this, "E1", "E2", "E3");
        subscription = EventBus.Subscribe(this, "E4");
        EventBus.Subscribe(this, "NoE4");
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

      public void E4()
      {
        E4Calls++;
      }

      public void NoE4()
      {
        subscription.Dispose();
      }

      //

      private readonly IDisposable subscription;
    }
  }
}