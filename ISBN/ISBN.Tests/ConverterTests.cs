using ISBN.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISBN.Tests
{
  [TestClass]
  public class ConverterTests
  {
    private Converter sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new Converter();
    }

    [TestMethod]
    public void Test1()
    {
      Check("978155192370", "155192370x");
    }

    [TestMethod]
    public void Test2()
    {
      Check("978140007917", "1400079179");
    }

    [TestMethod]
    public void Test3()
    {
      Check("978037541457", "0375414576");
    }

    [TestMethod]
    public void Test4()
    {
      Check("978037428158", "0374281580");
    }

    //

    private void Check(string given, string expected)
    {
      var result = sut.Convert(given);
      Assert.AreEqual(expected, result);
    }
  }
}