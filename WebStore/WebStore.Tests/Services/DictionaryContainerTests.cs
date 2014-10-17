using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Tests.Models;

namespace WebStore.Tests.Services
{
  [TestClass]
  public class DictionaryContainerTests
  {
    [TestMethod]
    public void FindsTheCorrectKey()
    {
      var sut = new DictionaryContainer<object>(null);
      var o = new object();
      sut.Register<SomeCommand>(() => o);

      var result = sut.Find<SomeCommand>();

      Assert.AreEqual(o, result);
    }

    [TestMethod]
    public void ReturnsTheDefaultValueWhenTheRequestedTypeHasNotBeenRegistered()
    {
      var def = new object();
      var sut = new DictionaryContainer<object>(def);

      var result = sut.Find<SomeCommand>();

      Assert.AreEqual(def, result);
    }
  }
}