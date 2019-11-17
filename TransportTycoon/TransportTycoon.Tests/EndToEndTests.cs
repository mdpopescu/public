using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransportTycoon.Library.Services;

namespace TransportTycoon.Tests
{
    [TestClass]
    [TestCategory("End to End")]
    public class EndToEndTests
    {
        private App app;

        [TestInitialize]
        public void SetUp()
        {
            app = new App();
        }

        [TestMethod]
        public void KnownResults()
        {
            Assert.AreEqual(5, app.Run("A"));
            Assert.AreEqual(5, app.Run("AB"));
            Assert.AreEqual(5, app.Run("BB"));
            Assert.AreEqual(7, app.Run("ABB"));
        }
    }
}