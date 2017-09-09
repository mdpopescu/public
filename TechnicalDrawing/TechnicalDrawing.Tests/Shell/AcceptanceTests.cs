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

            Assert.AreEqual(6, canvas.Commands.Count);
            Assert.AreEqual(new ProjectedCommand(Plane.XY, CommandName.Line, new QuadrantPoint(10, 20), new QuadrantPoint(40, 50)).ToString(), canvas.Commands[0]);
            Assert.AreEqual(new ProjectedCommand(Plane.XY, CommandName.Line, new QuadrantPoint(20, 30), new QuadrantPoint(50, 60)).ToString(), canvas.Commands[1]);
            Assert.AreEqual(new ProjectedCommand(Plane.XZ, CommandName.Line, new QuadrantPoint(10, 30), new QuadrantPoint(40, 60)).ToString(), canvas.Commands[2]);
            Assert.AreEqual(new ProjectedCommand(Plane.XZ, CommandName.Line, new QuadrantPoint(20, 40), new QuadrantPoint(50, 70)).ToString(), canvas.Commands[3]);
            Assert.AreEqual(new ProjectedCommand(Plane.YZ, CommandName.Line, new QuadrantPoint(20, 30), new QuadrantPoint(50, 60)).ToString(), canvas.Commands[4]);
            Assert.AreEqual(new ProjectedCommand(Plane.YZ, CommandName.Line, new QuadrantPoint(30, 40), new QuadrantPoint(60, 70)).ToString(), canvas.Commands[5]);
        }
    }
}