using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class SimpleLexerTests
  {
    [TestMethod]
    public void MultilineConstant()
    {
      var sut = new SimpleLexer();

      var result = sut.Tokenize("a\nb").ToList();

      Assert.AreEqual(2, result.Count);
      Assert.AreEqual("constant", result[0].Type);
      Assert.AreEqual("a\nb", result[0].Value);
      Assert.AreEqual(Token.EOF, result[1]);
    }

    [TestMethod]
    public void IncludeToken()
    {
      var sut = new SimpleLexer();

      var result = sut.Tokenize("a{{include other b}}c").ToList();

      Assert.AreEqual(4, result.Count);
      Assert.AreEqual("include", result[1].Type);
      Assert.AreEqual("{{include other b}}", result[1].Value);
    }
  }
}