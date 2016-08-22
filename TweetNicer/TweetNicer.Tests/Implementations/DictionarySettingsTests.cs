using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetNicer.Library.Implementations;

namespace TweetNicer.Tests.Implementations
{
    [TestClass]
    public class DictionarySettingsTests
    {
        private DictionarySettings sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new DictionarySettings();
        }

        [TestMethod]
        public void ReturnsNullForUnknownKeys()
        {
            var result = sut["a"];

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReturnsValueAssociatedWithAKey()
        {
            sut["a"] = "zzz";

            var result = sut["a"];

            Assert.AreEqual("zzz", result);
        }

        [TestMethod]
        public void KeysAreCaseInsensitive()
        {
            sut["a"] = "zzz";

            var result = sut["A"];

            Assert.AreEqual("zzz", result);
        }

        [TestMethod]
        public void GetKeysReturnsTheListOfKeys()
        {
            sut["a"] = "1";
            sut["B"] = "2";

            var result = sut.GetKeys().ToArray();

            CollectionAssert.AreEqual(new[] { "a", "B" }, result);
        }
    }
}