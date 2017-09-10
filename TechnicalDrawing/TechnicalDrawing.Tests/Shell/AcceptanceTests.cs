using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;
using TechnicalDrawing.Library.Shell;

namespace TechnicalDrawing.Tests.Shell
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void Test1()
        {
            var canvas = new FakeCanvas();
            var app = new DrawingApp(new WinFileSystem(), new Parser(), new Projector(), canvas);

            app.Load(@"Data\d1.txt");

            Assert.AreEqual(3, canvas.Commands.Count);
            CollectionAssert.AreEqual(new[]
            {
                new ProjectedCommand(Plane.XY, CommandName.Line, new Point2D(10, 20), new Point2D(40, 50)).ToString(),
                new ProjectedCommand(Plane.XY, CommandName.Line, new Point2D(20, 30), new Point2D(50, 60)).ToString(),
            }, canvas.Commands[0]);
            CollectionAssert.AreEqual(new[]
            {
                new ProjectedCommand(Plane.XZ, CommandName.Line, new Point2D(10, 30), new Point2D(40, 60)).ToString(),
                new ProjectedCommand(Plane.XZ, CommandName.Line, new Point2D(20, 40), new Point2D(50, 70)).ToString(),
            }, canvas.Commands[1]);
            CollectionAssert.AreEqual(new[]
            {
                new ProjectedCommand(Plane.YZ, CommandName.Line, new Point2D(20, 30), new Point2D(50, 60)).ToString(),
                new ProjectedCommand(Plane.YZ, CommandName.Line, new Point2D(30, 40), new Point2D(60, 70)).ToString(),
            }, canvas.Commands[2]);
        }
    }
}