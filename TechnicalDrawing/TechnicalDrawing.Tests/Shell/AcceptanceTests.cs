using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Shell;

namespace TechnicalDrawing.Tests.Shell
{
    [TestClass]
    public class AcceptanceTests
    {
        private FakeCanvas canvas;

        private DrawingApp sut;

        [TestInitialize]
        public void SetUp()
        {
            canvas = new FakeCanvas();

            sut = new DrawingApp(new FileParser(new WinFileSystem(), new LineParser()), new Projector(), canvas);
        }

        [TestMethod]
        public void Test1()
        {
            sut.Load(@"Data\d1.txt");

            Assert.AreEqual(3, canvas.Commands.Count);
            CollectionAssert.AreEqual(new[]
            {
                "XY Line (10, 20), (40, 50)",
                "XY Line (20, 30), (50, 60)",
            }, canvas.Commands[0]);
            CollectionAssert.AreEqual(new[]
            {
                "XZ Line (10, 30), (40, 60)",
                "XZ Line (20, 40), (50, 70)",
            }, canvas.Commands[1]);
            CollectionAssert.AreEqual(new[]
            {
                "YZ Line (20, 30), (50, 60)",
                "YZ Line (30, 40), (60, 70)",
            }, canvas.Commands[2]);
        }

        [TestMethod]
        public void Test2()
        {
            sut.Load(@"Data\d2.txt");

            Assert.AreEqual(3, canvas.Commands.Count);
            CollectionAssert.AreEqual(new[]
            {
                "XY Line (10, 20), (40, 50)",
                "XY Line (20, 30), (50, 60)",
            }, canvas.Commands[0]);
            CollectionAssert.AreEqual(new[]
            {
                "XZ Line (10, 30), (40, 60)",
                "XZ Line (20, 40), (50, 70)",
            }, canvas.Commands[1]);
            CollectionAssert.AreEqual(new[]
            {
                "YZ Line (20, 30), (50, 60)",
                "YZ Line (30, 40), (60, 70)",
            }, canvas.Commands[2]);
        }
    }
}