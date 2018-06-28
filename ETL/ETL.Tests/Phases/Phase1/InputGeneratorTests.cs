using System.Collections.Generic;
using System.Linq;
using ETL.Library.Phases.Phase1;
using ETL.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ETL.Tests.Phases.Phase1
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
            var spec = new InputSpec
            {
                Fields = new List<InputField>
                {
                    new InputField { Name = "first_name", Size = 30 },
                    new InputField { Name = "last_name", Size = 20 },
                    new InputField { Name = "date_of_birth", Size = 8 },
                }
            };

            var results = sut.Process(spec).ToList();

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(Resources.Phase1Test1Record, results[0]);
            Assert.AreEqual(Resources.Phase1Test1Extensions, results[1]);
        }
    }
}