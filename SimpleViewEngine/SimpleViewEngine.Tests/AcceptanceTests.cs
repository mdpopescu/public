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
      model.a = true;

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
    public void RepeaterWithSelf()
    {
      const string TEMPLATE = "{{foreach a}}-b-{{}}-c-{{endfor}}";
      model.a = new[] {1, 2, 3};

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("-b-1-c--b-2-c--b-3-c-", result);
    }

    [TestMethod]
    public void RepeaterWithProperties()
    {
      const string TEMPLATE = ">>{{foreach a}}-b-{{a}}-c-{{b}}-d-{{endfor}}<<";
      model.a = new[] {new {a = 1, b = "x"}, new {a = 2, b = "y"}};

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual(">>-b-1-c-x-d--b-2-c-y-d-<<", result);
    }

    [TestMethod]
    public void NestedRepeaters()
    {
      const string TEMPLATE = ">>{{foreach a}}-{{x}}-{{foreach c}}-y={{y}}-{{endfor}}-{{z}}-{{endfor}}<<";
      model.a = new[]
      {
        new {x = 11, c = new[] {new {y = 12}, new {y = 13}, new {y = 14}}, z = 15},
        new {x = 21, c = new[] {new {y = 22}, new {y = 23}, new {y = 24}}, z = 25},
      };

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual(">>-11--y=12--y=13--y=14--15--21--y=22--y=23--y=24--25-<<", result);
    }

    [TestMethod]
    [ExpectedException(typeof (KeyNotFoundException))]
    public void ThrowsIfPropertyInRepeaterDoesNotExist()
    {
      const string TEMPLATE = "{{foreach a}}-{{endfor}}";

      engine.Run(TEMPLATE, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsIfRepeaterIsNotClosed()
    {
      const string TEMPLATE = "{{foreach a}}";
      model.a = new[] {1, 2, 3};

      engine.Run(TEMPLATE, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsOnUnexpectedEndfor()
    {
      const string TEMPLATE = "{{endfor}}";

      engine.Run(TEMPLATE, model);
    }

    //

    private static Lexer CreateLexer()
    {
      var lexer = new Lexer();
      lexer.AddDefinition(new TokenDefinition("if", @"\{\{if \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endif", @"\{\{endif\}\}"));
      lexer.AddDefinition(new TokenDefinition("foreach", @"\{\{foreach \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endfor", @"\{\{endfor\}\}"));
      lexer.AddDefinition(new TokenDefinition("property", @"\{\{[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("constant", "[^{]+"));

      return lexer;
    }
  }
}