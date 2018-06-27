using System.Linq;
using ETL.Library.Implementations;
using ETL.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ETL.Tests.Implementations
{
    [TestClass]
    public class InputGeneratorTests
    {
        private InputGenerator sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new InputGenerator();
        }

        [TestMethod]
        public void SingleFixedSizeRecordWithFixedSizeFields()
        {
            const string spec = "\"\"first name\"\",30\r\n\"\"last name\"\",20\r\n\"\"date of birth\"\",8\r\n";

            var results = sut.Process(spec).ToList();

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Resources.Phase1Test1Record, results[0]);
            Assert.AreEqual(Resources.Phase1Test1Extensions, results[1]);
        }
    }
}