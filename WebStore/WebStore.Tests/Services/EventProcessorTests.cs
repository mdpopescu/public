using System.Collections.Generic;
using EventStore.Library.Contracts;
using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Tests.Models.Events;

namespace WebStore.Tests.Services
{
  [TestClass]
  public class EventProcessorTests
  {
    private Mock<AppendOnlyCollection<Event>> store;
    private Mock<Repository> repository;

    private EventProcessor sut;

    [TestInitialize]
    public void SetUp()
    {
      store = new Mock<AppendOnlyCollection<Event>>();
      repository = new Mock<Repository>();

      sut = new EventProcessor(store.Object, repository.Object);
    }

    [TestMethod]
    public void ReplaysTheEventHistoryOnStart()
    {
      var ev1 = new Mock<Event>();
      var ev2 = new Mock<Event>();
      var list = new List<Event> { ev1.Object, ev2.Object };
      store
        .Setup(it => it.Get())
        .Returns(list);

      sut.Start();

      ev1.Verify(it => it.Handle(repository.Object));
      ev2.Verify(it => it.Handle(repository.Object));
    }

    [TestMethod]
    public void CallsTheHandler()
    {
      var success = false;

      sut.Process(new SomeEvent(_ => { success = true; }));

      Assert.IsTrue(success);
    }
  }
}