using Conway.Library.Contracts;
using Conway.Library.Models;

namespace Conway.Library.Services
{
    public class Game
    {
        public Game(Board board, Screen screen)
        {
            this.board = board;
            this.screen = screen;
        }

        //

        private Board board;
        private Screen screen;
    }
}