using System.IO;
using System.Linq;
using System.Text;
using Inventory2.Library.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventory2.Tests.Shell
{
    [TestClass]
    public class WORMStreamTests
    {
        [TestMethod]
        public void ReadsBackTheDataThatWasWritten()
        {
            using (var ms = new MemoryStream())
            {
                var sut = new WORMStream(ms);

                sut.Append("some message");
                sut.Append("multiple\rlines\n");
                sut.Append(new byte[] { 1, 2, 3 });
                var results = sut.ReadAll().ToList();

                Assert.AreEqual("some message", Encoding.UTF8.GetString(results[0]));
                Assert.AreEqual("multiple\rlines\n", Encoding.UTF8.GetString(results[1]));
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, results[2]);
            }
        }
    }
}