using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Implementations;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void Test1()
        {
            var canvas = new Mock<Canvas>();
            var app = new DrawingApp(canvas.Object);

            app.Load(@"Data\d1.txt");

            canvas.Verify(it => it.Line(Quadrant.XY, new ScreenPoint(1, 2), new ScreenPoint(4, 5)));
            canvas.Verify(it => it.Line(Quadrant.XY, new ScreenPoint(2, 3), new ScreenPoint(5, 6)));
            canvas.Verify(it => it.Line(Quadrant.XZ, new ScreenPoint(1, 3), new ScreenPoint(4, 6)));
            canvas.Verify(it => it.Line(Quadrant.XZ, new ScreenPoint(2, 4), new ScreenPoint(5, 7)));
            canvas.Verify(it => it.Line(Quadrant.YZ, new ScreenPoint(2, 3), new ScreenPoint(5, 6)));
            canvas.Verify(it => it.Line(Quadrant.YZ, new ScreenPoint(3, 4), new ScreenPoint(6, 7)));
        }
    }
}