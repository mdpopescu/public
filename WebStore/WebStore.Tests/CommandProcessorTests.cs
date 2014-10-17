using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Tests.Models;

namespace WebStore.Tests
{
  [TestClass]
  public class CommandProcessorTests
  {
    [TestMethod]
    public void CallsTheProperHandler()
    {
      var sut = new CommandProcessor();
      var success = false;
      sut.Register<SomeCommand>(cmd =>
      {
        success = true;
        return null;
      });

      sut.Process(new SomeCommand());

      Assert.IsTrue(success);
    }
  }
}