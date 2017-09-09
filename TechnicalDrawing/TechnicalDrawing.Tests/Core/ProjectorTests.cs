using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Tests.Core
{
    [TestClass]
    public class ProjectorTests
    {
        private Projector sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new Projector();
        }

        [TestMethod]
        public void ProjectsOnePointToTheXYPlane()
        {
            var result = sut.Project(new ParsedCommand(CommandName.Line, 1.0f, 2.0f, 3.0f), Quadrant.XY);

            Assert.AreEqual(Quadrant.XY, result.Quadrant);
            Assert.AreEqual(1, result.Points.Length);
            Assert.AreEqual(new QuadrantPoint(1, 2), result.Points[0]);
        }
    }
}