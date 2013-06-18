using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Failover.Library;

namespace Renfield.Failover.Tests
{
  [TestClass]
  public class FailGuardTests
  {
    [TestInitialize]
    public void SetUp()
    {
      logger = new Mock<Logger>();
      sut = new FailGuard(logger.Object);
    }

    [TestClass]
    public class Attempt : FailGuardTests
    {
      [TestMethod]
      public void RunsSingleAction()
      {
        var success = false;

        sut.Attempt(() => { success = true; });

        Assert.IsTrue(success);
      }

      [TestMethod]
      public void DoesNothingIfThereAreNoActions()
      {
        sut.Attempt();

        // success - did not crash
      }

      [TestMethod]
      public void ContinuesToSecondActionIfTheFirstOneFails()
      {
        var success = false;

        sut.Attempt(
          () => { throw new Exception(); },
          () => { success = true; });

        Assert.IsTrue(success);
      }

      [TestMethod]
      public void LogsSuccessfulSingleAction()
      {
        sut.Attempt(() => { });

        logger.Verify(it => it.Debug("Success."));
      }

      [TestMethod]
      public void LogsError()
      {
        const string MESSAGE = "abc";

        sut.Attempt(() => { throw new Exception(MESSAGE); });

        logger.Verify(it => it.Error(It.Is<Exception>(e => e.Message == MESSAGE)));
      }

      [TestMethod]
      public void LogsErrorsAndThenSuccess()
      {
        var sequence = new MockSequence();
        logger.InSequence(sequence).Setup(it => it.Error((It.Is<Exception>(e => e.Message == "1")))).Verifiable();
        logger.InSequence(sequence).Setup(it => it.Error((It.Is<Exception>(e => e.Message == "2")))).Verifiable();
        logger.InSequence(sequence).Setup(it => it.Error((It.Is<Exception>(e => e.Message == "3")))).Verifiable();
        logger.InSequence(sequence).Setup(it => it.Debug("Success")).Verifiable();
        
        sut.Attempt(
          () => { throw new Exception("1"); },
          () => { throw new Exception("2"); },
          () => { throw new Exception("3"); },
          () => { });

        logger.Verify();
      }

      [TestMethod]
      public void LogsOnlyErrors()
      {
        var sequence = new MockSequence();
        logger.InSequence(sequence).Setup(it => it.Error((It.Is<Exception>(e => e.Message == "1")))).Verifiable();
        logger.InSequence(sequence).Setup(it => it.Error((It.Is<Exception>(e => e.Message == "2")))).Verifiable();
        logger.InSequence(sequence).Setup(it => it.Error((It.Is<Exception>(e => e.Message == "3")))).Verifiable();

        sut.Attempt(
          () => { throw new Exception("1"); },
          () => { throw new Exception("2"); },
          () => { throw new Exception("3"); });

        logger.Verify();
      }
    }

    //

    private Mock<Logger> logger;
    private FailGuard sut;
  }
}