using System.Collections.Generic;
using EventStore.Library.Contracts;
using EventStore.Library.Models;
using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebStore.Tests.Services
{
  [TestClass]
  public class EventProcessorTests
  {
    [TestMethod]
    public void ReplaysTheEventHistoryOnStart()
    {
      var ev1 = new Mock<Event>();
      var ev2 = new Mock<Event>();
      var list = new List<Event> { ev1.Object, ev2.Object };
      var store = new Mock<AppendOnlyCollection<Event>>();
      store
        .Setup(it => it.Get())
        .Returns(list);
      var repository = new Mock<Repository>();
      var sut = new EventProcessor(store.Object, repository.Object);

      sut.Start();

      ev1.Verify(it => it.Handle(repository.Object));
      ev2.Verify(it => it.Handle(repository.Object));
    }
  }
}