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
        private Mock<IOutput> output;

        private App sut;

        [TestInitialize]
        public void SetUp()
        {
            fs = new Mock<IFileSystem>();
            codeGenerator = new Mock<ICodeGenerator>();
            output = new Mock<IOutput>();

            sut = new App(fs.Object, codeGenerator.Object, output.Object);
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

                sut.GenerateDecorator(filename);

                output.Verify(it => it.WriteCode(newCode));
            }
        }
    }
}