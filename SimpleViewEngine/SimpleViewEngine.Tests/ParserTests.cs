using System;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
      private SimpleViewParser sut;

      [TestInitialize]
      public void SetUp()
      {
        model = new ExpandoObject();
        sut = new SimpleViewParser();
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
        var result = sut.Parse(new[] {new Token("constant", "test", null)}).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConstantNode));
        Assert.AreEqual("test", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsOnePropertyNodeWithCorrectValue()
      {
        model.a = "x";

        var result = sut.Parse(new[] {new Token("property", "{{a}}", null),}).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("x", result[0].Eval(model));
      }

      [TestMethod]
      public void PropertyNodeEvaluatesSelf()
      {
        model = 123;

        var result = sut.Parse(new[] {new Token("property", "{{}}", null),}).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("123", result[0].Eval(model));
      }

      [TestMethod]
      public void EofTokenStopsTheParsing()
      {
        model.a = "x";

        var result = sut.Parse(new[]
        {
          new Token("property", "{{a}}", null),
          new Token("(eof)", null, null),
          new Token("constant", "test", null),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("x", result[0].Eval(model));
      }

      [TestMethod]
      [ExpectedException(typeof (Exception))]
      public void UnknownTokenThrows()
      {
        sut.Parse(new[] {new Token("unknown", "abc", null),}).ToList();
      }

      [TestMethod]
      public void ReturnsConditionalNode()
      {
        model.b = true;

        var result = sut.Parse(new[]
        {
          new Token("if", "{{if b}}", null),
          new Token("constant", "test", null),
          new Token("endif", "{{endif}}", null),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsConditionalNodeWithElseCase1()
      {
        model.b = true;

        var result = sut.Parse(new[]
        {
          new Token("if", "{{if b}}", null),
          new Token("constant", "test1", null),
          new Token("else", "{{else}}", null),
          new Token("constant", "test2", null),
          new Token("endif", "{{endif}}", null),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test1", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsConditionalNodeWithElseCase2()
      {
        model.b = false;

        var result = sut.Parse(new[]
        {
          new Token("if", "{{if b}}", null),
          new Token("constant", "test1", null),
          new Token("else", "{{else}}", null),
          new Token("constant", "test2", null),
          new Token("endif", "{{endif}}", null),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test2", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsRepeaterNode()
      {
        model.a = new[] {1, 2, 3};

        var result = sut.Parse(new[]
        {
          new Token("foreach", "{{foreach a}}", null),
          new Token("constant", "test", null),
          new Token("endfor", "{{endfor}}", null),
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (RepeaterNode));
        Assert.AreEqual("testtesttest", result[0].Eval(model));
      }
    }
  }
}