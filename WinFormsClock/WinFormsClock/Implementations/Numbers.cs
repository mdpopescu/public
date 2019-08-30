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

                    var center = PolarCoord.Create(degree, radius * 0.80f).CarthesianCoord.Offset(origin);
                    var size = Math.Max(24.0f, radius * 0.15f); // don't let the numbers get too small

                    var location = center.Offset(new PointF(-size / 2, -size / 2));

                    // hour 0 should be 12
                    var sHour = hour == 0 ? "12" : hour.ToString();
                    var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                    g.DrawString(sHour, SystemFonts.DefaultFont, brush, new RectangleF(location, new SizeF(size, size)), format);
                }
            }
        }
    }
}