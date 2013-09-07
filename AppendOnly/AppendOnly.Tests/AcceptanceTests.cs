using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using Renfield.AppendOnly.Library;

namespace Renfield.AppendOnly.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void GenericAppendOnlyFileSmokeTest()
    {
      var stream = new MemoryStream();
      var data = new StreamAccessor(stream);
      var file = new LowLevelAppendOnlyFile(data);
      var serializer = new ProtoBufSerializationEngine();
      var sut = new GenericAppendOnlyFile<TestClass>(file, serializer);

      sut.Append(new TestClass {Name = "Marcel", Address = "Kennesaw, GA"});
      sut.Append(new TestClass {Name = "Gigi Meseriasu", Address = "Washington, DC"});

      var r1 = sut.Read(0);
      Assert.AreEqual("Marcel", r1.Name);
      Assert.AreEqual("Kennesaw, GA", r1.Address);

      var rs = sut.ReadFrom(0).ToList();
      var r2 = rs[1];
      Assert.AreEqual("Gigi Meseriasu", r2.Name);
      Assert.AreEqual("Washington, DC", r2.Address);

      Assert.AreEqual(2, sut.Index.Length);
      Assert.AreEqual(0, sut.Index[0]);
    }

    //

    [ProtoContract]
    private class TestClass
    {
      [ProtoMember(1)]
      public string Name { get; set; }

      [ProtoMember(2)]
      public string Address { get; set; }
    }
  }
}