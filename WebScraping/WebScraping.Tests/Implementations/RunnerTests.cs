using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebScraping.Library.Implementations;
using WebScraping.Library.Interfaces;

namespace WebScraping.Tests.Implementations
{
    [TestClass]
    public class RunnerTests
    {
        private Mock<Compiler> compiler;
        private Mock<Interpreter> interpreter;

        private Runner sut;

        [TestInitialize]
        public void SetUp()
        {
            compiler = new Mock<Compiler>();
            interpreter = new Mock<Interpreter>();

            sut = new Runner(compiler.Object, interpreter.Object);
        }

        [TestMethod]
        public void RunsAnEmptyProgramWithoutErrors()
        {
            var sb = new StringBuilder();
            using (var env = ObjectMother.CreateEnvironment("", sb))
            {
                sut.Run("", env);
            }
        }

        [TestMethod]
        public void CompilesTheProgram()
        {
            var sb = new StringBuilder();
            using (var env = ObjectMother.CreateEnvironment("", sb))
            {
                sut.Run("print 'a string'", env);

                compiler.Verify(it => it.Compile("print 'a string'"));
            }
        }

        [TestMethod]
        public void RunsTheCompiledProgram()
        {
            var sb = new StringBuilder();
            using (var env = ObjectMother.CreateEnvironment("", sb))
            {
                compiler
                    .Setup(it => it.Compile("print 'a string'"))
                    .Returns("//1//\r\nConsole.WriteLine(\"a string\");\r\n");

                sut.Run("print 'a string'", env);

                // ReSharper disable once AccessToDisposedClosure
                interpreter.Verify(it => it.Run("//1//\r\nConsole.WriteLine(\"a string\");\r\n", env));
            }
        }
    }
}