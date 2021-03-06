﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
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
      serializer = new Mock<Serializer<LicenseRegistration>>();
      encryptor = new Mock<Encryptor>();
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
      public void ReturnsNullIfInputIsEmpty()
      {
        io
          .Setup(it => it.Read())
          .Returns("  ");

        sut.Load();
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
      public void ReturnsNullIfTheDecryptorThrows()
      {
        io
          .Setup(it => it.Read())
          .Returns("abc");
        encryptor
          .Setup(it => it.Decrypt("abc"))
          .Throws(new Exception());

        var result = sut.Load();

        Assert.IsNull(result);
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
        var details = new LicenseRegistration();
        serializer
          .Setup(it => it.Deserialize("def"))
          .Returns(details);

        var result = sut.Load();

        Assert.AreEqual(details, result);
      }

      [TestMethod]
      public void ReturnsNullIfTheSerializerThrows()
      {
        io
          .Setup(it => it.Read())
          .Returns("abc");
        encryptor
          .Setup(it => it.Decrypt("abc"))
          .Returns("def");
        serializer
          .Setup(it => it.Deserialize("def"))
          .Throws(new Exception());

        var result = sut.Load();

        Assert.IsNull(result);
      }
    }

    [TestClass]
    public class Save : SecureStorageTests
    {
      [TestMethod]
      public void SerializesTheObject()
      {
        var details = new LicenseRegistration();

        sut.Save(details);

        serializer.Verify(it => it.Serialize(details));
      }

      [TestMethod]
      public void EncryptsTheString()
      {
        var details = new LicenseRegistration();
        serializer
          .Setup(it => it.Serialize(details))
          .Returns("abc");

        sut.Save(details);

        encryptor.Verify(it => it.Encrypt("abc"));
      }

      [TestMethod]
      public void WritesTheStringToTheOutput()
      {
        var details = new LicenseRegistration();
        serializer
          .Setup(it => it.Serialize(details))
          .Returns("abc");
        encryptor
          .Setup(it => it.Encrypt("abc"))
          .Returns("def");

        sut.Save(details);

        io.Verify(it => it.Write("def"));
      }
    }

    [TestClass]
    public class Delete : SecureStorageTests
    {
      [TestMethod]
      public void WritesAnEmptyValueToTheOutput()
      {
        sut.Delete();

        io.Verify(it => it.Write(""));
      }
    }
  }
}