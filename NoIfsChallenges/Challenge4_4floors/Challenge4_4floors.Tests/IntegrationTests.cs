using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge4_4floors.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private List<string> events;

        [TestInitialize]
        public void SetUp()
        {
            events = new List<string>();
        }

        [TestMethod]
        public void Starting()
        {
            Assert.Fail();
        }
    }
}