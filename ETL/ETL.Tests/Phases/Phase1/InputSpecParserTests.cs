using ETL.Library.Phases.Phase1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ETL.Tests.Phases.Phase1
{
    [TestClass]
    public class InputSpecParserTests
    {
        private InputSpecParser sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new InputSpecParser();
        }

        [TestMethod]
        public void ParsesSpec()
        {
            const string spec = "\"first name\",30\r\n\"last name\",20\r\n\"date of birth\",8\r\n";

            var result = sut.Parse(spec);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Fields.Count);
            Assert.AreEqual("first_name", result.Fields[0].Name);
            Assert.AreEqual(30, result.Fields[0].Size);
            Assert.AreEqual("last_name", result.Fields[1].Name);
            Assert.AreEqual(20, result.Fields[1].Size);
            Assert.AreEqual("date_of_birth", result.Fields[2].Name);
            Assert.AreEqual(8, result.Fields[2].Size);
        }
    }
}