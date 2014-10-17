using EventStore.Library.Contracts;
using EventStore.Library.Models;
using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Tests.Models;

namespace WebStore.Tests.Services
{
  [TestClass]
  public class CommandProcessorTests
  {
    private Mock<Processor<Event>> next;

    private CommandProcessor sut;

    [TestInitialize]
    public void SetUp()
    {
      next = new Mock<Processor<Event>>();

      sut = new CommandProcessor(next.Object);
    }

    [TestMethod]
    public void CallsTheProperHandler()
    {
      var success = false;
      sut.Register<SomeCommand>(cmd =>
      {
        success = true;
        return null;
      });

      sut.Process(new SomeCommand());

      Assert.IsTrue(success);
    }

    [TestMethod]
    public void CallsTheNextLinkInTheChain()
    {
      var ev = new SomeEvent();
      sut.Register<SomeCommand>(_ => ev);

      sut.Process(new SomeCommand());

      next.Verify(it => it.Process(ev));
    }

    [TestMethod]
    public void DoesNotCallTheNextLinkIfUnknownCommand()
    {
      sut.Process(new SomeCommand());

      next.Verify(it => it.Process(It.IsAny<Event>()), Times.Never);
    }

    //[TestMethod]
    //public void DoesNotCallTheNextLinkIfAnErrorIsThrown()
    //{
    //  Assert.Fail();
    //}
  }
}