using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Helpers;
using Renfield.SimpleViewEngine.Library.Parsing;
using Renfield.SimpleViewEngine.Library.Properties;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private Lexer lexer;
    private SimpleParser parser;
    private Engine engine;
    private dynamic model;

    [TestInitialize]
    public void SetUp()
    {
      lexer = new SimpleLexer();
      parser = new SimpleParser(ParsingRules.Create);
      engine = new Engine();
      model = new ExpandoObject();
    }

    [TestMethod]
    public void ConstantString()
    {
      const string TEMPLATE = "testing";
      var nodes = Parse(TEMPLATE);

      var result = engine.Run(nodes, model);

      Assert.AreEqual(TEMPLATE, result);
    }

    [TestMethod]
    public void ConstantStringWithCurlyBraces()
    {
      const string TEMPLATE = "testing { non-property }} sequences";
      var nodes = Parse(TEMPLATE);

      var result = engine.Run(nodes, model);

      Assert.AreEqual(TEMPLATE, result);
    }

    [TestMethod]
    public void PropertySubstitutions()
    {
      const string TEMPLATE = "c1 {{v1}} c2 {{v2}} c3";
      var nodes = Parse(TEMPLATE);
      model.v1 = "abc";
      model.v2 = "def";

      var result = engine.Run(nodes, model);

      Assert.AreEqual("c1 abc c2 def c3", result);
    }

    [TestMethod]
    public void SubProperty()
    {
      const string TEMPLATE = "c1 {{v.a}} c2 {{v.b}} c3";
      var nodes = Parse(TEMPLATE);
      model.v = new {a = "123", b = "456"};

      var result = engine.Run(nodes, model);

      Assert.AreEqual("c1 123 c2 456 c3", result);
    }

    [TestMethod]
    [ExpectedException(typeof (KeyNotFoundException))]
    public void ThrowsIfPropertyDoesNotExist()
    {
      const string TEMPLATE = "{{a}}";
      var nodes = Parse(TEMPLATE);

      engine.Run(nodes, model);
    }

    [TestMethod]
    public void Conditionals()
    {
      const string TEMPLATE = "a-{{if b}}-c-{{endif}}-d-{{if e}}-f-{{endif}}-g";
      var nodes = Parse(TEMPLATE);
      model.b = true;
      model.e = false;

      var result = engine.Run(nodes, model);

      Assert.AreEqual("a--c--d--g", result);
    }

    [TestMethod]
    public void NestedConditionals()
    {
      const string TEMPLATE = "a-{{if b}}-c-{{if d}}-e-{{endif}}-f-{{if g}}-h-{{endif}}-i-{{endif}}-j";
      var nodes = Parse(TEMPLATE);
      model.b = true;
      model.d = true;
      model.g = false;

      var result = engine.Run(nodes, model);

      Assert.AreEqual("a--c--e--f--i--j", result);
    }

    [TestMethod]
    public void ConditionalWithElse()
    {
      const string TEMPLATE = "a{{if b}}c{{else}}d{{endif}}e";
      var nodes = Parse(TEMPLATE).ToList();

      model.b = true;
      var r1 = engine.Run(nodes, model);

      model.b = false;
      var r2 = engine.Run(nodes, model);

      var result = r1 + "-" + r2;

      Assert.AreEqual("ace-ade", result);
    }

    [TestMethod]
    [ExpectedException(typeof (KeyNotFoundException))]
    public void ThrowsIfPropertyInConditionalDoesNotExist()
    {
      const string TEMPLATE = "{{if a}}-{{endif}}";
      var nodes = Parse(TEMPLATE);

      engine.Run(nodes, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsIfConditionalIsNotClosed()
    {
      const string TEMPLATE = "{{if a}}";
      var nodes = Parse(TEMPLATE);
      model.a = true;

      engine.Run(nodes, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsOnUnexpectedEndif()
    {
      const string TEMPLATE = "{{endif}}";
      var nodes = Parse(TEMPLATE);

      engine.Run(nodes, model);
    }

    [TestMethod]
    public void RepeaterWithSelf()
    {
      const string TEMPLATE = "{{foreach a}}-b-{{}}-c-{{endfor}}";
      var nodes = Parse(TEMPLATE);
      model.a = new[] {1, 2, 3};

      var result = engine.Run(nodes, model);

      Assert.AreEqual("-b-1-c--b-2-c--b-3-c-", result);
    }

    [TestMethod]
    public void RepeaterWithProperties()
    {
      const string TEMPLATE = ">>{{foreach a}}-b-{{a}}-c-{{b}}-d-{{endfor}}<<";
      var nodes = Parse(TEMPLATE);
      model.a = new[] {new {a = 1, b = "x"}, new {a = 2, b = "y"}};

      var result = engine.Run(nodes, model);

      Assert.AreEqual(">>-b-1-c-x-d--b-2-c-y-d-<<", result);
    }

    [TestMethod]
    public void NestedRepeaters()
    {
      const string TEMPLATE = ">>{{foreach a}}-{{x}}-{{foreach c}}-y={{y}}-{{endfor}}-{{z}}-{{endfor}}<<";
      var nodes = Parse(TEMPLATE);
      model.a = new[]
      {
        new {x = 11, c = new[] {new {y = 12}, new {y = 13}, new {y = 14}}, z = 15},
        new {x = 21, c = new[] {new {y = 22}, new {y = 23}, new {y = 24}}, z = 25},
      };

      var result = engine.Run(nodes, model);

      Assert.AreEqual(">>-11--y=12--y=13--y=14--15--21--y=22--y=23--y=24--25-<<", result);
    }

    [TestMethod]
    [ExpectedException(typeof (KeyNotFoundException))]
    public void ThrowsIfPropertyInRepeaterDoesNotExist()
    {
      const string TEMPLATE = "{{foreach a}}-{{endfor}}";
      var nodes = Parse(TEMPLATE);

      engine.Run(nodes, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsIfRepeaterIsNotClosed()
    {
      const string TEMPLATE = "{{foreach a}}";
      var nodes = Parse(TEMPLATE);
      model.a = new[] {1, 2, 3};

      engine.Run(nodes, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void ThrowsOnUnexpectedEndfor()
    {
      const string TEMPLATE = "{{endfor}}";
      var nodes = Parse(TEMPLATE);

      engine.Run(nodes, model);
    }

    [TestMethod]
    [ExpectedException(typeof (Exception))]
    public void UnbalancedConditionalsAndRepeaters()
    {
      const string TEMPLATE = "{{if a}}-{{foreach b}}-{{endif}}-{{endfor}}";
      var nodes = Parse(TEMPLATE);
      model.a = true;
      model.b = new[] {1, 2, 3};

      engine.Run(nodes, model);
    }

    [TestMethod]
    public void Html()
    {
      var template = Resources.HtmlFragment;
      var nodes = Parse(template);
      model.Title = "Index";

      var result = engine.Run(nodes, model);

      var expected = template.Replace("{{Title}}", "Index");
      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void IncludeOtherTemplate()
    {
      const string MAIN_TEMPLATE = "12{{a}}45{{include other b}}67{{c}}89";
      const string OTHER_TEMPLATE = "{{x}} and {{y}}";

      var mainNodes = Parse(MAIN_TEMPLATE);
      var otherNodes = Parse(OTHER_TEMPLATE);

      model.a = "a";
      model.b = new {x = "x", y = "y"};
      model.c = "c";

      engine.ParseTemplate = name => otherNodes;

      var result = engine.Run(mainNodes, model);

      Assert.AreEqual("12a45x and y67c89", result);
    }

    [TestMethod]
    public void ConditionalInclude()
    {
      Assert.Fail("Not implemented");
    }

    //

    private IEnumerable<Node> Parse(string template)
    {
      var tokens = lexer
        .Tokenize(template)
        .ToTokenList();

      return parser.Parse(tokens);
    }
  }
}