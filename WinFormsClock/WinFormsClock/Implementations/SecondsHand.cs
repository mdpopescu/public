using System;
using System.Drawing;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    internal class SecondsHand : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;
        public float Width { get; set; } = 1.0f;

        public void Draw(Graphics g, Point origin, int radius)
        {
            using (var pen = new Pen(Color, Width))
            {
                var time = DateTime.Now.TimeOfDay;

                var second = (float) time.TotalSeconds;
                var degree = second * 6;

                var lineStart = PolarCoord.Create(degree, radius * 0.05f).CarthesianCoord.Offset(origin);
                var lineEnd = PolarCoord.Create(degree, radius * 0.85f).CarthesianCoord.Offset(origin);

                g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
            }
        }
    }
}