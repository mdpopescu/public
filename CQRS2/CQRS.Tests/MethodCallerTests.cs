using System.Diagnostics;
using System.Threading;
using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class MethodCallerTests
  {
    [TestMethod]
    public void CallsTheMethodAsynchronously()
    {
      var obj = new MyClass1();
      var method = obj.GetType().GetMethod("Method1");

      var sw = new Stopwatch();
      sw.Start();
      MethodCaller.Call(method, obj);
      sw.Stop();

      Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
    }

    [TestMethod]
    public void AnExceptionInTheCalledMethodCallsTheExceptionHandler()
    {
      try
      {
        var called = false;
        MethodCaller.UnhandledException = _ => { called = true; };
        var obj = new MyClass1();
        var method = obj.GetType().GetMethod("Method2");

        MethodCaller.Call(method, obj);

        Thread.Sleep(10);
        Assert.IsTrue(called);
      }
      finally
      {
        MethodCaller.UnhandledException = null;
      }
    }

    [TestMethod]
    public void TerminatesTheApplicationIfAnExceptionIsThrownAndNoHandlerIsDefined()
    {
      var terminated = false;
      WinSystem.Terminate = () => terminated = true;
      var obj = new MyClass1();
      var method = obj.GetType().GetMethod("Method2");

      MethodCaller.Call(method, obj);

      Thread.Sleep(10);
      Assert.IsTrue(terminated);
    }

    //

    private class MyClass1
    {
      public void Method1()
      {
        Thread.Sleep(2000);
      }
    }
  }
}