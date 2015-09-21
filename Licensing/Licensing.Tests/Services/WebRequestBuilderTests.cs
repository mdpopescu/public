using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class WebRequestBuilderTests
  {
    private Mock<Sys> sys;

    private WebRequestBuilder sut;

    [TestInitialize]
    public void SetUp()
    {
      sys = new Mock<Sys>();

      sut = new WebRequestBuilder(sys.Object, "{key}/{pid}");
    }

    [TestClass]
    public class BuildQuery : WebRequestBuilderTests
    {
      [TestMethod]
      public void ReplacesTheKeyPlaceholderWithTheRegistrationKey()
      {
        var registration = ObjectMother.CreateRegistration();

        var result = sut.BuildQuery(registration);

        Assert.IsTrue(result.Contains(ObjectMother.KEY));
      }

      [TestMethod]
      public void ReplacesThePidPlaceholderWithTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("123abc");

        var result = sut.BuildQuery(registration);

        Assert.IsTrue(result.Contains("123abc"));
      }
    }

    [TestClass]
    public class BuildData : WebRequestBuilderTests
    {
      [TestMethod]
      public void EncodesTheRegistrationDetails()
      {
        List<KeyValuePair<string, string>> list = null;

        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("123abc");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Callback<IEnumerable<KeyValuePair<string, string>>>(r => list = r.ToList());

        sut.BuildData(registration);

        Assert.IsTrue(list.Any(p => p.Key == "Key" && p.Value == ObjectMother.KEY));
        Assert.IsTrue(list.Any(p => p.Key == "Name" && p.Value == ObjectMother.NAME));
        Assert.IsTrue(list.Any(p => p.Key == "Contact" && p.Value == ObjectMother.CONTACT));
        // also check for the processor id
        Assert.IsTrue(list.Any(p => p.Key == "ProcessorId" && p.Value == "123abc"));
      }
    }
  }
}