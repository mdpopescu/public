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
  }
}