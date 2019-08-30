using System;
using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    internal class Numbers : IClockPart
    {
        public Color Color { get; set; } = Color.Red;

        public void Draw(Graphics g, Point origin, int radius)
        {
            using (var brush = new SolidBrush(Color))
            {
                foreach (var hour in Enumerable.Range(0, 12))
                {
                    var degree = hour * 30;

                    var center = PolarCoord.Create(degree, radius * 0.85f).CarthesianCoord.Offset(origin);
                    var size = Math.Max(24.0f, radius * 0.15f); // don't let the numbers get too small

                    var location = center.Offset(new PointF(-size / 2, -size / 2));

                    // hour 0 should be 12
                    var sHour = hour == 0 ? "12" : hour.ToString();
                    var font = GetAdjustedFont(g, "12", new Font("Arial", 36), size, 36, 5);
                    var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                    g.DrawString(sHour, font, brush, new RectangleF(location, new SizeF(size, size)), format);
                }
            }
        }

        //

        private static Font GetAdjustedFont(Graphics g, string text, Font font, float width, int maxSize, int minSize)
        {
            var goodSize = Enumerable
                .Range(minSize, maxSize - minSize + 1)
                .Reverse()
                .Where(emSize => GetTextSize(g, text, font, emSize) <= width)
                .FirstOrDefault();
            if (goodSize < minSize)
                goodSize = minSize;
            return new Font(font.Name, goodSize, font.Style);
        }

        private static float GetTextSize(Graphics g, string text, Font font, int emSize)
        {
            using (var testFont = new Font(font.Name, emSize, font.Style))
                return g.MeasureString(text, testFont).Width;
        }
    }
}