using Conway.Library.Contracts;
using Conway.Library.Models;
using Conway.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Conway.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private Mock<Screen> screen;

        private Board board;

        [TestInitialize]
        public void SetUp()
        {
            screen = new Mock<Screen>();

            board = new Board(screen.Object, new Size(5, 5));
        }

        [TestMethod]
        public void Test1()
        {
            board.Set(new Coordinates(2, 1));
            board.Set(new Coordinates(2, 2));
            board.Set(new Coordinates(2, 3));

            screen.Verify(it => it.Clear());
            screen.Verify(it => it.ShowBlock(new Coordinates(2, 1)));
            screen.Verify(it => it.ShowBlock(new Coordinates(2, 2)));
            screen.Verify(it => it.ShowBlock(new Coordinates(2, 3)));

            board.Round();

            screen.Verify(it => it.Clear(), Times.Exactly(2));
            screen.Verify(it => it.ShowBlock(new Coordinates(1, 2)));
            screen.Verify(it => it.ShowBlock(new Coordinates(2, 2)));
            screen.Verify(it => it.ShowBlock(new Coordinates(3, 2)));
        }
    }
}