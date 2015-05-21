﻿using System;
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
    private Mock<Repository> repository;
    private CommandHandler sut;

    [TestInitialize]
    public void SetUp()
    {
      repository = new Mock<Repository>();
      sut = new CommandHandler(repository.Object);
    }

    [TestClass]
    public class Post : CommandHandlerTests
    {
      [TestMethod]
      public void SavesMessageToRepository()
      {
        Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);

        sut.Post("Alice", "message");

        repository.Verify(it => it.Add(It.Is<Message>(m =>
          m.CreatedOn == new DateTime(2000, 1, 2, 3, 4, 5) &&
          m.User == "Alice" &&
          m.Text == "message")));
      }
    }
  }
}