using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TechnicalDrawing.Library.Contracts;
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
            var canvas = new Mock<Canvas>();
            var app = new DrawingApp(new WinFileSystem(), new Parser(), new Projector(), canvas.Object);

            app.Load(@"Data\d1.txt");

            canvas.Verify(it => it.Execute(new ProjectedCommand(CommandName.Line, Quadrant.XY, new QuadrantPoint(10, 20), new QuadrantPoint(40, 50))));
            canvas.Verify(it => it.Execute(new ProjectedCommand(CommandName.Line, Quadrant.XY, new QuadrantPoint(20, 30), new QuadrantPoint(50, 60))));
            canvas.Verify(it => it.Execute(new ProjectedCommand(CommandName.Line, Quadrant.XZ, new QuadrantPoint(10, 30), new QuadrantPoint(40, 60))));
            canvas.Verify(it => it.Execute(new ProjectedCommand(CommandName.Line, Quadrant.XZ, new QuadrantPoint(20, 40), new QuadrantPoint(50, 70))));
            canvas.Verify(it => it.Execute(new ProjectedCommand(CommandName.Line, Quadrant.YZ, new QuadrantPoint(20, 30), new QuadrantPoint(50, 60))));
            canvas.Verify(it => it.Execute(new ProjectedCommand(CommandName.Line, Quadrant.YZ, new QuadrantPoint(30, 40), new QuadrantPoint(60, 70))));
        }
    }
}