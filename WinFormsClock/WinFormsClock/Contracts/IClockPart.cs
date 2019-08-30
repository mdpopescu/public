using System.Drawing;

namespace WinFormsClock.Contracts
{
    public interface IClockPart
    {
        Color Color { get; set; }

        void Draw(Graphics g, Point origin, int radius);
    }
}