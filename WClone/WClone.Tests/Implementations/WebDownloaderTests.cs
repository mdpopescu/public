using Microsoft.VisualStudio.TestTools.UnitTesting;
using WClone.Library.Implementations;

namespace WClone.Tests.Implementations
{
    [TestClass]
    public class WebDownloaderTests
    {
        private WebDownloader sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new WebDownloader();
        }
    }
}