using System;
using EventStore.Library.Contracts;
using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Tests.Models;

namespace WebStore.Tests.Services
{
  [TestClass]
  public class CommandProcessorTests
  {
    private Mock<Repository> repository;
    private Mock<Processor<Event>> next;

    private CommandProcessor sut;

    [TestInitialize]
    public void SetUp()
    {
      repository = new Mock<Repository>();
      next = new Mock<Processor<Event>>();

      sut = new CommandProcessor(repository.Object, next.Object);
    }

    [TestMethod]
    public void CallsTheHandler()
    {
      var success = false;

      sut.Process(new SomeCommand(_ =>
      {
        success = true;
        return null;
      }));

      Assert.IsTrue(success);
    }

    [TestMethod]
    public void CallsTheNextLinkInTheChain()
    {
      var ev = new SomeEvent();

      sut.Process(new SomeCommand(_ => ev));

      next.Verify(it => it.Process(ev));
    }

    [TestMethod]
    public void DoesNotCallTheNextLinkIfAnErrorIsThrown()
    {
      try
      {
        sut.Process(new SomeCommand(_ => { throw new Exception(); }));
      }
      catch
      {
        // do nothing
      }

      next.Verify(it => it.Process(It.IsAny<Event>()), Times.Never);
    }
  }
}