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
            var result = sut.Project(Plane.XY, 1.0f, 2.0f, 3.0f);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(new QuadrantPoint(1, 2), result[0]);
        }

        [TestMethod]
        public void ProjectsOnePointToTheXZPlane()
        {
            var result = sut.Project(Plane.XZ, 1.0f, 2.0f, 3.0f);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(new QuadrantPoint(1, 3), result[0]);
        }

        [TestMethod]
        public void ProjectsOnePointToTheYZPlane()
        {
            var result = sut.Project(Plane.YZ, 1.0f, 2.0f, 3.0f);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(new QuadrantPoint(2, 3), result[0]);
        }
    }
}