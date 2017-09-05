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
        public void ParsesPost()
        {
            var result = sut.Parse("user -> message");

            Assert.AreEqual("user", result.User);
            Assert.AreEqual("->", result.Command);
            Assert.AreEqual("message", result.Rest);
        }

        [TestMethod]
        public void ParsesRead()
        {
            var result = sut.Parse("user");

            Assert.AreEqual("user", result.User);
            Assert.AreEqual("", result.Command);
        }

        [TestMethod]
        public void ParsesFollows()
        {
            var result = sut.Parse("user1 follows user2");

            Assert.AreEqual("user1", result.User);
            Assert.AreEqual("FOLLOWS", result.Command);
            Assert.AreEqual("user2", result.Rest);
        }

        [TestMethod]
        public void ParsesWall()
        {
            var result = sut.Parse("user wall");

            Assert.AreEqual("user", result.User);
            Assert.AreEqual("WALL", result.Command);
        }
    }
}