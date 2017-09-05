using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Tests.Logic
{
    [TestClass]
    public class MessageParserTests
    {
        private MessageParser sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MessageParser();
        }

        [TestMethod]
        public void ParsesPostingLine()
        {
            var result = sut.Parse("user -> message");

            Assert.AreEqual("user", result.User);
            Assert.AreEqual("->", result.Command);
            Assert.AreEqual("message", result.Rest);
        }

        [TestMethod]
        public void ParsesReadingLine()
        {
            var result = sut.Parse("user");

            Assert.AreEqual("user", result.User);
            Assert.AreEqual("", result.Command);
            Assert.IsNull(result.Rest);
        }
    }
}