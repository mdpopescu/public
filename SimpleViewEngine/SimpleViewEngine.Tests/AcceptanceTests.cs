using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library;

namespace Renfield.SimpleViewEngine.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void ConstantString()
    {
      const string TEMPLATE = "testing";
      dynamic model = "";
      var engine = new Engine(new Parser());

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
      var engine = new Engine(new Parser());

      var result = engine.Run(TEMPLATE, model);

      Assert.AreEqual("c1 abc c2 def c3", result);
    }
  }
}