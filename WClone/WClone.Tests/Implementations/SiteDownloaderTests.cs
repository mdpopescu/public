using Microsoft.VisualStudio.TestTools.UnitTesting;
using WClone.Library.Implementations;

namespace WClone.Tests.Implementations
{
    [TestClass]
    public class SiteDownloaderTests
    {
        private SiteDownloader sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new SiteDownloader();
        }
    }
}