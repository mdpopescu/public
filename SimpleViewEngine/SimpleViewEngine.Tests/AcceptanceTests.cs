using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private ILexer lexer;
    private Parser parser;
    private Engine engine;
    private dynamic model;

    [TestInitialize]
    public void SetUp()
    {
      lexer = CreateLexer();
      parser = new Parser();
      engine = new Engine(lexer, parser);
      model = new ExpandoObject();
    }

    [TestMethod]
    public void ConstantString()
    {
      const string TEMPLATE = "testing";

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual(TEMPLATE, result);
    }

    [TestMethod]
    public void PropertySubstitutions()
    {
      const string TEMPLATE = "c1 {{v1}} c2 {{v2}} c3";
      model.v1 = "abc";
      model.v2 = "def";

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("c1 abc c2 def c3", result);
    }

    [TestMethod]
    [ExpectedException(typeof (KeyNotFoundException))]
    public void ThrowsIfPropertyDoesNotExist()
    {
      const string TEMPLATE = "{{a}}";

      engine.Run(TEMPLATE, model);
    }

    [TestMethod]
    public void Conditionals()
    {
      const string TEMPLATE = "a-{{if b}}-c-{{endif}}-d-{{if e}}-f-{{endif}}-g";
      model.b = true;
      model.e = false;

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("a--c--d--g", result);
    }

    [TestMethod]
    public void NestedConditionals()
    {
      const string TEMPLATE = "a-{{if b}}-c-{{if d}}-e-{{endif}}-f-{{if g}}-h-{{endif}}-i-{{endif}}-j";
      model.b = true;
      model.d = true;
      model.g = false;

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("a--c--e--f--i--j", result);
    }

    [TestMethod]
    [ExpectedException(typeof (KeyNotFoundException))]
    public void ThrowsIfPropertyInConditionalDoesNotExist()
    {
      const string TEMPLATE = "{{if a}}-{{endif}}";

      engine.Run(TEMPLATE, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsIfConditionalIsNotClosed()
    {
      const string TEMPLATE = "{{if a}}";

      engine.Run(TEMPLATE, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsOnUnexpectedEndif()
    {
      const string TEMPLATE = "{{endif}}";

      engine.Run(TEMPLATE, model);
    }

    [TestMethod]
    public void Repeater()
    {
      const string TEMPLATE = "{{foreach a}}-b-{{it}}-c-{{endfor}}";
      model.a = new[] {1, 2, 3};

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("-b-1-c--b-2-c--b-3-c-", result);
    }

    //

    private static Lexer CreateLexer()
    {
      var lexer = new Lexer();
      lexer.AddDefinition(new TokenDefinition("if", @"\{\{if \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endif", @"\{\{endif\}\}"));
      lexer.AddDefinition(new TokenDefinition("foreach", @"\{\{foreach \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endfor", @"\{\{endfor\}\}"));
      lexer.AddDefinition(new TokenDefinition("property", @"\{\{\w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("constant", "[^{]+"));

      return lexer;
    }
  }
}