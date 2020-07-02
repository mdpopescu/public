using AutoBogus;
using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class AppTests
    {
        private Mock<IFileSystem> fs;
        private Mock<ICodeGenerator> codeGenerator;
        private Mock<IFilenameGenerator> filenameGenerator;

        private App sut;

        [TestInitialize]
        public void SetUp()
        {
            fs = new Mock<IFileSystem>();
            codeGenerator = new Mock<ICodeGenerator>();
            filenameGenerator = new Mock<IFilenameGenerator>();

            sut = new App(fs.Object, codeGenerator.Object, filenameGenerator.Object);
        }

        [TestClass]
        public class GenerateDecorator : AppTests
        {
            [TestMethod]
            public void LoadsTheFile()
            {
                var filename = AutoFaker.Generate<string>();

                sut.GenerateDecorator(filename);

                fs.Verify(it => it.ReadText(filename));
            }

            [TestMethod]
            public void InvokesTheGenerator()
            {
                var filename = AutoFaker.Generate<string>();
                var code = AutoFaker.Generate<string>();
                fs.Setup(it => it.ReadText(filename)).Returns(code);

                sut.GenerateDecorator(filename);

                codeGenerator.Verify(it => it.Generate(code));
            }

            [TestMethod]
            public void SavesTheResult()
            {
                var filename = AutoFaker.Generate<string>();
                var code = AutoFaker.Generate<string>();
                fs.Setup(it => it.ReadText(filename)).Returns(code);
                var newCode = AutoFaker.Generate<string>();
                codeGenerator.Setup(it => it.Generate(code)).Returns(newCode);
                var newFile = AutoFaker.Generate<string>();
                filenameGenerator.Setup(it => it.Generate(filename)).Returns(newFile);

                sut.GenerateDecorator(filename);

                fs.Verify(it => it.WriteText(newFile, newCode));
            }
        }
    }
}