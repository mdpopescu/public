using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class HandsOrigin : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;

        public void Draw(Graphics g, Point origin, int radius)
        {
            using (var brush = new SolidBrush(Color))
            {
                var size = radius * 0.05f;
                var rect = new RectangleF(origin.X - size, origin.Y - size, size * 2, size * 2);

                g.FillEllipse(brush, rect);
            }
        }
    }
}