using System;
using Acta.Library.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acta.Tests.Models
{
  [TestClass]
  public class ActaTupleTests
  {
    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ConstructorRejectsInvalidPropertyName()
    {
      var temp = new ActaTuple(Guid.NewGuid(), "  ", "value");
    }
  }
}