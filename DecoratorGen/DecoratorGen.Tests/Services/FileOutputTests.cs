using AutoBogus;
using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class FileOutputTests
    {
        private Mock<IFilenameGenerator> filenameGenerator;
        private Mock<IFileSystem> fs;

        private FileOutput sut;

        [TestInitialize]
        public void SetUp()
        {
            filenameGenerator = new Mock<IFilenameGenerator>();
            fs = new Mock<IFileSystem>();

            sut = new FileOutput(filenameGenerator.Object, fs.Object);
        }

        [TestClass]
        public class WriteCode : FileOutputTests
        {
            [TestMethod]
            public void RequestsTheFilenameFromTheGenerator()
            {
                var code = AutoFaker.Generate<string>();

                sut.WriteCode(code);

                filenameGenerator.Verify(it => it.Generate(code));
            }

            [TestMethod]
            public void WritesTheCodeToTheFile()
            {
                var code = AutoFaker.Generate<string>();
                var filename = AutoFaker.Generate<string>();
                filenameGenerator.Setup(it => it.Generate(code)).Returns(filename);

                sut.WriteCode(code);

                fs.Verify(it => it.WriteText(filename, code));
            }
        }
    }
}