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
  }
}