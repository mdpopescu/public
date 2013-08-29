using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private ILexer lexer;
    private Parser parser;
    private Engine engine;

    [TestInitialize]
    public void SetUp()
    {
      lexer = CreateLexer();
      parser = new Parser();
      engine = new Engine(lexer, parser);
    }

    [TestMethod]
    public void ConstantString()
    {
      const string TEMPLATE = "testing";
      dynamic model = "";

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual(TEMPLATE, result);
    }

    [TestMethod]
    public void PropertySubstitutions()
    {
      const string TEMPLATE = "c1 {{v1}} c2 {{v2}} c3";
      dynamic model = new ExpandoObject();
      model.v1 = "abc";
      model.v2 = "def";

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("c1 abc c2 def c3", result);
    }

    //

    private static Lexer CreateLexer()
    {
      var lexer = new Lexer();
      lexer.AddDefinition(new TokenDefinition("property", @"\{\{\w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("constant", "[^{]*"));

      return lexer;
    }
  }
}