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
            var result = sut.Project(Plane.XY, new ParsedCommand(CommandName.Line, 1.0f, 2.0f, 3.0f));

            Assert.AreEqual(Plane.XY, result.Plane);
            Assert.AreEqual(1, result.Points.Length);
            Assert.AreEqual(new QuadrantPoint(1, 2), result.Points[0]);
        }

        [TestMethod]
        public void ProjectsOnePointToTheXZPlane()
        {
            var result = sut.Project(Plane.XZ, new ParsedCommand(CommandName.Line, 1.0f, 2.0f, 3.0f));

            Assert.AreEqual(Plane.XZ, result.Plane);
            Assert.AreEqual(1, result.Points.Length);
            Assert.AreEqual(new QuadrantPoint(1, 3), result.Points[0]);
        }

        [TestMethod]
        public void ProjectsOnePointToTheYZPlane()
        {
            var result = sut.Project(Plane.YZ, new ParsedCommand(CommandName.Line, 1.0f, 2.0f, 3.0f));

            Assert.AreEqual(Plane.YZ, result.Plane);
            Assert.AreEqual(1, result.Points.Length);
            Assert.AreEqual(new QuadrantPoint(2, 3), result.Points[0]);
        }
    }
}