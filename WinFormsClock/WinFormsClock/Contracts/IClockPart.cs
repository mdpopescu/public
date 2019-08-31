using System.Drawing;

namespace WinFormsClock.Contracts
{
    internal interface IClockPart
    {
        Color Color { get; set; }

        void Draw(Graphics g, Point origin, int size);
        void Draw(ICanvas canvas);
    }
}