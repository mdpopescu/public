using System;
using System.Collections.Generic;
using System.IO;
using BigDataProcessing.Library.Contracts;
using BigDataProcessing.Library.Models;
using BigDataProcessing.Library.Services;
using BigDataProcessing.Tests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TextReader = BigDataProcessing.Library.Contracts.TextReader;

namespace BigDataProcessing.Tests.Services
{
  [TestClass]
  public class AppTests
  {
    private Mock<Logger> logger;
    private Mock<TextReader> reader;
    private Mock<RxTextWriter> writer;
    private Mock<LineConverter> processor;

    private App sut;

    [TestInitialize]
    public void SetUp()
    {
      logger = new Mock<Logger>();
      reader = new Mock<TextReader>();
      writer = new Mock<RxTextWriter>();
      processor = new Mock<LineConverter>();

      sut = new App(logger.Object, reader.Object, writer.Object, new[] { processor.Object });
    }

    [TestMethod]
    public void ReadsFromTheInput()
    {
      var input = new object();
      var config = new Configuration { Input = input };

      sut.Run(config);

      reader.Verify(it => it.Read(input));
    }

    [TestMethod]
    public void LogsLoadingErrors()
    {
      sut.Run(new Configuration
      {
        Input = new MemoryStream(),
        Output = new MemoryStream(),
      });

      logger.Verify(it => it.Log("Error reading from the input."));
    }

    [TestMethod]
    public void ProcessesTheContents()
    {
      var input = new object();
      var config = new Configuration { Input = input };
      var contents = new[] { "a" };
      reader
        .Setup(it => it.Read(input))
        .Returns(contents);
      writer
        .Setup(it => it.Write(It.IsAny<object>(), It.IsAny<IObservable<string>>()))
        .Callback<object, IObservable<string>>((_, obs) => obs.Subscribe(__ => { }));

      sut.Run(config);

      processor.Verify(it => it.Convert("a"));
    }

    [TestMethod]
    public void WritesTheResults()
    {
      var input = new object();
      var config = new Configuration { Input = input };
      var contents = new[] { "a", "b", "c" };
      reader
        .Setup(it => it.Read(input))
        .Returns(contents);
      processor
        .Setup(it => it.Convert(It.IsAny<string>()))
        .Returns<string>(it => (it[0] - 'a' + 1).ToString());
      List<string> list = null;
      writer
        .Setup(it => it.Write(It.IsAny<object>(), It.IsAny<IObservable<string>>()))
        .Callback<object, IObservable<string>>((_, obs) => list = Extensions.ToList(obs));

      sut.Run(config);

      Assert.AreEqual(3, list.Count);
      Assert.AreEqual("1", list[0]);
      Assert.AreEqual("2", list[1]);
      Assert.AreEqual("3", list[2]);
    }

    [TestMethod]
    public void SkipsLinesThatCannotBeConverted()
    {
      var input = new object();
      var config = new Configuration { Input = input };
      var contents = new[] { "a", "error" };
      reader
        .Setup(it => it.Read(input))
        .Returns(contents);
      processor
        .Setup(it => it.Convert("a"))
        .Returns("b");
      var list = new List<string>();
      writer
        .Setup(it => it.Write(It.IsAny<object>(), It.IsAny<IObservable<string>>()))
        .Callback<object, IObservable<string>>((_, obs) => obs.Subscribe(list.Add))
        .Verifiable();

      sut.Run(config);

      writer.Verify();
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual("b", list[0]);
    }
  }
}