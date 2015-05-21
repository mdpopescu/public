using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork.Library.Contracts;
using SocialNetwork.Library.Models;
using SocialNetwork.Library.Services;

namespace SocialNetwork.Tests.Services
{
  [TestClass]
  public class CommandHandlerTests
  {
    private Mock<MessageRepository> messages;
    private Mock<UserRepository> users;
    private CommandHandler sut;

    [TestInitialize]
    public void SetUp()
    {
      messages = new Mock<MessageRepository>();
      users = new Mock<UserRepository>();
      sut = new CommandHandler(messages.Object, users.Object, new TimeFormatter());
    }

    [TestClass]
    public class Post : CommandHandlerTests
    {
      [TestMethod]
      public void SavesMessageToRepository()
      {
        Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);

        sut.Post("Alice", "message");

        messages.Verify(it => it.Add(It.Is<Message>(m =>
          m.CreatedOn == new DateTime(2000, 1, 2, 3, 4, 5) &&
          m.User == "Alice" &&
          m.Text == "message")));
      }
    }

    [TestClass]
    public class Read : CommandHandlerTests
    {
      [TestMethod]
      public void ReadsMessagesBelongingToGivenUser()
      {
        messages
          .Setup(it => it.Get())
          .Returns(new[] { new Message(new DateTime(2000, 1, 2, 3, 4, 5), "Alice", "message") });

        var result = sut.Read("Alice").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].StartsWith("message"));
      }

      [TestMethod]
      public void ReturnsTheTimeSinceTheMessageWasPosted_1Second()
      {
        Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 6);
        messages
          .Setup(it => it.Get())
          .Returns(new[] { new Message(new DateTime(2000, 1, 2, 3, 4, 5), "Alice", "message") });

        var result = sut.Read("Alice").ToList();

        Assert.AreEqual("message (1 second ago)", result[0]);
      }

      [TestMethod]
      public void ReturnsTheMessagesInReverseOrder()
      {
        Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 10);
        messages
          .Setup(it => it.Get())
          .Returns(new[]
          {
            new Message(new DateTime(2000, 1, 2, 3, 4, 5), "Alice", "message1"),
            new Message(new DateTime(2000, 1, 2, 3, 4, 9), "Alice", "message2"),
          });

        var result = sut.Read("Alice").ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("message2 (1 second ago)", result[0]);
        Assert.AreEqual("message1 (5 seconds ago)", result[1]);
      }
    }

    [TestClass]
    public class Follow : CommandHandlerTests
    {
      [TestMethod]
      public void AddsOtherUserToListOfFollowedUsers()
      {
        sut.Follow("Alfa", "Beta");

        users.Verify(it => it.AddFollower("Alfa", "Beta"));
      }
    }
  }
}