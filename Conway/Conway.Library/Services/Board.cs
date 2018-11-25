using System;
using Conway.Library.Contracts;
using Conway.Library.Models;

namespace Conway.Library.Services
{
    public class Board
    {
        public Board(Screen screen, Size size)
        {
            this.screen = screen;
            this.size = size;
        }

        public void Set(Coordinates coordinates)
        {
            throw new NotImplementedException();
        }

        public void Round()
        {
            throw new NotImplementedException();
        }

        //

        private Screen screen;
        private Size size;
    }
}