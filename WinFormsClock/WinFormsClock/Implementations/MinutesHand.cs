using System;
using System.Drawing;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    internal class MinutesHand : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;
        public float Width { get; set; } = 1.0f;

        public void Draw(Graphics g, Point origin, int size)
        {
            using (var pen = new Pen(Color, Width))
            {
                var time = DateTime.Now.TimeOfDay;

                var minute = (float) time.TotalMinutes;
                var degree = minute * 6;

                var lineStart = PolarCoord.Create(degree, size * 0.05f).CarthesianCoord.Offset(origin);
                var lineEnd = PolarCoord.Create(degree, size * 0.70f).CarthesianCoord.Offset(origin);

                g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
            }
        }
    }
}