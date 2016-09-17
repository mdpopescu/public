using Elomen.Library.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elomen.Tests.Implementations
{
    [TestClass]
    public class MonitorTests
    {
        private Monitor sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new Monitor();
        }
    }
}