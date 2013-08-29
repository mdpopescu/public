using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class ParserTests
  {
    [TestClass]
    public class Parse : ParserTests
    {
      [TestMethod]
      public void ReturnsEmptyEnumerable()
      {
        var sut = new Parser();

        var result = sut.Parse("").ToList();

        Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public void ReturnsOneConstantNode()
      {
        var sut = new Parser();

        var result = sut.Parse("test").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConstantNode));
      }
    }
  }
}