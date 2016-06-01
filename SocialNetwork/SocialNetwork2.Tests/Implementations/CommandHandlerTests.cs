using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Tests.Implementations
{
    [TestClass]
    public class CommandHandlerTests
    {
        private UserRepository userRepository;

        private CommandHandler sut;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new UserRepository();
            sut = new CommandHandler(userRepository);
        }

        [TestMethod]
        public void PostingAddsMessagesToUser()
        {
            sut.Handle("abc -> test");

            var user = userRepository.CreateOrFind("abc");
            var messages = user.Read().ToList();
            Assert.AreEqual(1, messages.Count);
            Assert.IsTrue(messages[0].StartsWith("test"));
        }

        [TestMethod]
        public void ReadingReturnsMessagesFromTheUser()
        {
            sut.Handle("abc -> test");

            var result = sut.Handle("abc").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].StartsWith("test"));
        }
    }
}