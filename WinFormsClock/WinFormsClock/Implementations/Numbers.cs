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

        public void Draw(Graphics g, Point origin, int size)
        {
            using (var brush = new SolidBrush(Color))
            {
                foreach (var hour in Enumerable.Range(0, 12))
                {
                    var degree = hour * 30;

                    var center = PolarCoord.Create(degree, size * 0.85f).CarthesianCoord.Offset(origin);

                    var drawingSize = Math.Max(24.0f, size * 0.15f); // don't let the numbers get too small
                    var drawingRect = new RectangleF(center.X - drawingSize / 2, center.Y - drawingSize / 2, drawingSize, drawingSize);

                    // hour 0 should be 12
                    var sHour = hour == 0 ? "12" : hour.ToString();
                    var font = GetAdjustedFont(g, "12", new Font("Arial", 36), drawingSize, 36, 5);
                    var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                    g.DrawString(sHour, font, brush, drawingRect, format);
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