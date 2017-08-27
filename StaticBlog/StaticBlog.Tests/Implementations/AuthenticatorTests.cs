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
        public void ReturnsTrueImmediatelyWhenTheCredentialsAreCorrect()
        {
            var account = new Account("marcel", "123456");

            var result = sut.Login(account);

            Assert.IsTrue(result);
            clock.Verify(it => it.Sleep(It.IsAny<TimeSpan>()), Times.Never);
        }

        [TestMethod]
        public void SleepsForOneSecondBeforeReturningFalseIfTheCredentialsAreWrong()
        {
            var account = new Account("marcel", "wrong");

            var result = sut.Login(account);

            Assert.IsFalse(result);
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        public void DoublesTheSleepTimeForUpToAMinuteIfTheCredentialsAreWrong()
        {
            var account = new Account("marcel", "wrong");

            sut.Login(account);
            sut.Login(account);
            sut.Login(account);
            sut.Login(account);
            sut.Login(account);
            sut.Login(account);
            sut.Login(account);
            sut.Login(account);

            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(1)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(2)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(4)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(8)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(16)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(32)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(60)));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(60)));
        }

        [TestMethod]
        public void ResetsTheSleepTimeIfTheTheCredentialsAreCorrect()
        {
            sut.Login(new Account("marcel", "wrong")); // sleeps 1 sec
            sut.Login(new Account("marcel", "wrong")); // sleeps 2 sec
            sut.Login(new Account("marcel", "123456")); // does not sleep, resets the time to sleep to 1 sec
            sut.Login(new Account("marcel", "wrong")); // sleeps 1 sec

            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(1)), Times.Exactly(2));
            clock.Verify(it => it.Sleep(TimeSpan.FromSeconds(2)));
        }
    }
}