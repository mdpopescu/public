using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    internal class MinuteMarks : IClockPart
    {
        public Color Color { get; set; } = Color.Black;

        public void Draw(Graphics g, Point origin, int size)
        {
            using (var pen = new Pen(Color))
            {
                foreach (var minute in Enumerable.Range(0, 60))
                {
                    var degree = minute * 6;

                    var lineStart = PolarCoord.Create(degree, size * 0.90f).CarthesianCoord.Offset(origin);
                    var lineEnd = PolarCoord.Create(degree, size * 0.95f).CarthesianCoord.Offset(origin);

                    g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
                }
            }
        }
    }
}