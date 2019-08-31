using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class Background : IClockPart
    {
        public Color Color { get; set; } = Color.LightBlue;

        public void Draw(Graphics g, Point origin, int size)
        {
            g.Clear(Color);
        }
    }
}