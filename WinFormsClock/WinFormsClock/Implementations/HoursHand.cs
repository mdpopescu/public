using System;
using System.Drawing;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    internal class HoursHand : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;
        public float Width { get; set; } = 1.0f;

        public void Draw(Graphics g, Point origin, int size)
        {
            using (var pen = new Pen(Color, Width))
            {
                var time = DateTime.Now.TimeOfDay;
                var hour = (float) time.TotalHours % 12;
                var degree = hour * 30;

                var lineStart = PolarCoord.Create(degree, size * 0.05f).CarthesianCoord.Offset(origin);
                var lineEnd = PolarCoord.Create(degree, size * 0.50f).CarthesianCoord.Offset(origin);

                g.DrawLine(pen, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
            }
        }

        public void Draw(ICanvas canvas)
        {
            var time = DateTime.Now.TimeOfDay;
            var hour = (float) time.TotalHours % 12;
            var degree = hour * 30;

            canvas.Line(Color, Width, canvas.Point(degree, 0.05f), canvas.Point(degree, 0.50f));
        }
    }
}