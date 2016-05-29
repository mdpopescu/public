using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebScraping.Library.Implementations;
using WebScraping.Library.Interfaces;

namespace WebScraping.Tests.Implementations
{
    [TestClass]
    public class MultiStepCompilerTests
    {
        [TestMethod]
        public void ReturnsNullIfProgramIsEmpty()
        {
            var sut = new MultiStepCompiler();

            var result = sut.Compile("  ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ChecksAllStatementCompilersToFindOneThatCanHandleTheStatement()
        {
            var compiler = new Mock<StatementCompiler>();
            var sut = new MultiStepCompiler(compiler.Object);

            sut.Compile("abc");

            compiler.Verify(it => it.CanHandle(new[] { "abc" }));
        }

        [TestMethod]
        public void OnceASuitableStatementCompilerWasFoundTheRestAreNotVerified()
        {
            var compiler1 = new Mock<StatementCompiler>();
            compiler1
                .Setup(it => it.CanHandle(new[] { "abc" }))
                .Returns(true);
            var compiler2 = new Mock<StatementCompiler>();
            var sut = new MultiStepCompiler(compiler1.Object, compiler2.Object);

            sut.Compile("abc");

            compiler2.Verify(it => it.CanHandle(new[] { "abc" }), Times.Never);
        }

        [TestMethod]
        public void TheStatementCompilerThatWasFoundIsAskedForATranslation()
        {
            var compiler1 = new Mock<StatementCompiler>();
            compiler1
                .Setup(it => it.CanHandle(new[] { "abc" }))
                .Returns(true);
            var compiler2 = new Mock<StatementCompiler>();
            var sut = new MultiStepCompiler(compiler1.Object, compiler2.Object);

            sut.Compile("abc");

            compiler1.Verify(it => it.Compile(new[] { "abc" }));
            compiler2.Verify(it => it.Compile(new[] { "abc" }), Times.Never);
        }

        [TestMethod]
        public void DoesNotCheckCommentLines()
        {
            var compiler = new Mock<StatementCompiler>();
            var sut = new MultiStepCompiler(compiler.Object);

            sut.Compile("// comment");

            compiler.Verify(it => it.CanHandle(It.IsAny<string[]>()), Times.Never);
        }
    }
}