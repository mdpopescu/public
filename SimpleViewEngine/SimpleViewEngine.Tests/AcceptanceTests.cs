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
    public void Conditionals_True()
    {
      const string TEMPLATE = "a1-{{if b1}}-c1-{{endif}}-d1";
      model.b1 = true;

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("a1--c1--d1", result);
    }

    //

    private static Lexer CreateLexer()
    {
      var lexer = new Lexer();
      lexer.AddDefinition(new TokenDefinition("if", @"\{\{if \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endif", @"\{\{endif\}\}"));
      lexer.AddDefinition(new TokenDefinition("property", @"\{\{\w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("constant", "[^{]+"));

      return lexer;
    }
  }
}