using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using Renfield.AppendOnly.Library;
using Renfield.AppendOnly.Library.Services;

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
            // the protobuf serializer might not be thread-safe
            var safeSerializer = new ConcurrentSerializationEngine(serializer);
            var sut = new GenericAppendOnlyFile<TestClass>(file, safeSerializer);

            sut.Append(new TestClass { Name = "Marcel", Address = "Kennesaw, GA" });
            sut.Append(new TestClass { Name = "Gigi Meseriasu", Address = "Washington, DC" });

            var r1 = sut.Read(0);
            Assert.AreEqual("Marcel", r1.Name);
            Assert.AreEqual("Kennesaw, GA", r1.Address);

            var rs = sut.ReadFrom(0).ToList();
            var r2 = rs[1];
            Assert.AreEqual("Gigi Meseriasu", r2.Name);
            Assert.AreEqual("Washington, DC", r2.Address);

            var index = sut.GetIndex();
            Assert.AreEqual(2, index.Length);
            Assert.AreEqual(0, index[0]);
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