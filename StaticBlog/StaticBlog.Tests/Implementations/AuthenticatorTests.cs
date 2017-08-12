using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StaticBlog.Library.Contracts;
using StaticBlog.Library.Implementations;
using StaticBlog.Library.Models;

namespace StaticBlog.Tests.Implementations
{
    [TestClass]
    public class AuthenticatorTests
    {
        private Mock<SystemClock> clock;

        private Authenticator sut;

        [TestInitialize]
        public void SetUp()
        {
            clock = new Mock<SystemClock>();

            sut = new Authenticator(clock.Object);
        }

        [TestMethod]
        public void ReturnsTrueImmediatelyWhenTheUsernameAndPasswordAreCorrect()
        {
            var account = KnownAccounts.List[0];

            var result = sut.Login(account.Key, account.Value);

            Assert.IsTrue(result);
            clock.Verify(it => it.Sleep(It.IsAny<TimeSpan>()), Times.Never);
        }
    }
}