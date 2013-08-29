using System;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class ParserTests
  {
    [TestClass]
    public class Parse : ParserTests
    {
      private dynamic model;
      private Parser sut;

      [TestInitialize]
      public void SetUp()
      {
        model = new ExpandoObject();
        sut = new Parser();
      }

      [TestMethod]
      public void ReturnsEmptyEnumerable()
      {
        var result = sut.Parse(Enumerable.Empty<Token>()).ToList();

        Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public void ReturnsOneConstantNodeWithCorrectValue()
      {
        var result = sut.Parse(new[] {new Token("constant", "test", new TokenPosition(0, 0, 0))}).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConstantNode));
        Assert.AreEqual("test", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsOnePropertyNodeWithCorrectValue()
      {
        model.a = "x";

        var result = sut.Parse(new[] {new Token("property", "{{a}}", new TokenPosition(0, 0, 0)),}).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("x", result[0].Eval(model));
      }

      [TestMethod]
      public void EofTokenStopsTheParsing()
      {
        model.a = "x";

        var result = sut.Parse(new[]
        {
          new Token("property", "{{a}}", new TokenPosition(0, 0, 0)),
          new Token("(eof)", null, new TokenPosition(5, 0, 5)),
          new Token("constant", "test", new TokenPosition(6, 0, 6)),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("x", result[0].Eval(model));
      }

      [TestMethod]
      [ExpectedException(typeof (Exception))]
      public void UnknownTokenThrows()
      {
        sut.Parse(new[] {new Token("unknown", "abc", new TokenPosition(0, 0, 0)),}).ToList();
      }

      [TestMethod]
      public void ReturnsConditionalNode()
      {
        model.b = true;

        var result = sut.Parse(new[]
        {
          new Token("if", "{{if b}}", new TokenPosition(0, 0, 0)),
          new Token("constant", "test", new TokenPosition(8, 0, 8)),
          new Token("endif", "{{endif}}", new TokenPosition(12, 0, 12)),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test", result[0].Eval(model));
      }
    }
  }
}