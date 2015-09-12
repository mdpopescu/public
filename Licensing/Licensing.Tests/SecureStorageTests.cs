﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests
{
  [TestClass]
  public class SecureStorageTests
  {
    private Mock<StringIO> io;
    private Mock<Encryptor> encryptor;
    private Mock<Serializer<LicenseRegistration>> serializer;
    private SecureStorage sut;

    [TestInitialize]
    public void SetUp()
    {
      io = new Mock<StringIO>();
      encryptor = new Mock<Encryptor>();
      serializer = new Mock<Serializer<LicenseRegistration>>();
      sut = new SecureStorage(io.Object, encryptor.Object, serializer.Object);
    }

    [TestClass]
    public class Load : SecureStorageTests
    {
      [TestMethod]
      public void ReadsTheStringFromTheInput()
      {
        sut.Load();

        io.Verify(it => it.Read());
      }

      [TestMethod]
      public void DecryptsTheString()
      {
        io
          .Setup(it => it.Read())
          .Returns("abc");

        sut.Load();

        encryptor.Verify(it => it.Decrypt("abc"));
      }

      [TestMethod]
      public void DeserializesTheString()
      {
        io
          .Setup(it => it.Read())
          .Returns("abc");
        encryptor
          .Setup(it => it.Decrypt("abc"))
          .Returns("def");

        sut.Load();

        serializer.Verify(it => it.Deserialize("def"));
      }

      [TestMethod]
      public void ReturnsTheDeserializedObject()
      {
        io
          .Setup(it => it.Read())
          .Returns("abc");
        encryptor
          .Setup(it => it.Decrypt("abc"))
          .Returns("def");
        var expected = new LicenseRegistration();
        serializer
          .Setup(it => it.Deserialize("def"))
          .Returns(expected);

        var result = sut.Load();

        Assert.AreEqual(expected, result);
      }
    }

    [TestClass]
    public class Save : SecureStorageTests
    {
      //
    }
  }
}