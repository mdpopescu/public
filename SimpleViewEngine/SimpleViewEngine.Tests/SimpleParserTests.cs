using System;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class SimpleParserTests
  {
    [TestClass]
    public class Parse : SimpleParserTests
    {
      private dynamic model;
      private SimpleParser sut;

      [TestInitialize]
      public void SetUp()
      {
        model = new ExpandoObject();
        sut = new SimpleParser(ParsingRules.Create);
      }

      [TestMethod]
      public void ReturnsEmptyEnumerable()
      {
        var result = sut.Parse(ObjectMother.CreateEmptyList()).ToList();

        Assert.AreEqual(0, result.Count);
      }

      [TestMethod]
      public void ReturnsOneConstantNodeWithCorrectValue()
      {
        var result = sut.Parse(ObjectMother.CreateConstantNode()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConstantNode));
        Assert.AreEqual("test", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsOnePropertyNodeWithCorrectValue()
      {
        model.a = "x";

        var result = sut.Parse(ObjectMother.CreateOnePropertyNode()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("x", result[0].Eval(model));
      }

      [TestMethod]
      public void PropertyNodeEvaluatesSelf()
      {
        model = 123;

        var result = sut.Parse(ObjectMother.CreateOnePropertyNodeEvaluatingSelf()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("123", result[0].Eval(model));
      }

      [TestMethod]
      public void EofTokenStopsTheParsing()
      {
        model.a = "x";

        var result = sut.Parse(ObjectMother.CreateEofInMiddle()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (PropertyNode));
        Assert.AreEqual("x", result[0].Eval(model));
      }

      [TestMethod]
      [ExpectedException(typeof (Exception))]
      public void UnknownTokenThrows()
      {
        sut.Parse(ObjectMother.CreateUnknownToken());
      }

      [TestMethod]
      public void ReturnsConditionalNode()
      {
        model.b = true;

        var result = sut.Parse(ObjectMother.CreateIf()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsConditionalNodeWithElseCase1()
      {
        model.b = true;

        var result = sut.Parse(ObjectMother.CreateIfElse()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test1", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsConditionalNodeWithElseCase2()
      {
        model.b = false;

        var result = sut.Parse(ObjectMother.CreateIfElse()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (ConditionalNode));
        Assert.AreEqual("test2", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsRepeaterNode()
      {
        model.a = new[] {1, 2, 3};

        var result = sut.Parse(ObjectMother.CreateForEach()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (RepeaterNode));
        Assert.AreEqual("testtesttest", result[0].Eval(model));
      }

      [TestMethod]
      public void ReturnsIncludeNode()
      {
        var result = sut.Parse(ObjectMother.CreateInclude()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result[0], typeof (IncludeNode));
      }
    }
  }
}