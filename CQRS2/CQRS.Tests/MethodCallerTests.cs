using System.Threading.Tasks;
using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class MethodCallerTests
  {
    [TestMethod]
    public async Task AnExceptionInTheCalledMethodCallsTheExceptionHandler()
    {
      try
      {
        var called = false;
        MethodCaller.UnhandledException = _ => { called = true; };
        var obj = new object();
        var method = obj.GetType().GetMethod("Method");

        await MethodCaller.CallAsync(method, obj);

        Assert.IsTrue(called);
      }
      finally
      {
        MethodCaller.UnhandledException = null;
      }
    }

    [TestMethod]
    public async Task TerminatesTheApplicationIfAnExceptionIsThrownAndNoHandlerIsDefined()
    {
      var terminated = false;
      WinSystem.Terminate = () => terminated = true;
      var obj = new object();
      var method = obj.GetType().GetMethod("Method");

      await MethodCaller.CallAsync(method, obj);

      Assert.IsTrue(terminated);
    }
  }
}