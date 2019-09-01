using System;
using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class MinutesHand : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;
        public float Width { get; set; } = 1.0f;

        public void Draw(ICanvas canvas)
        {
            var time = DateTime.Now.TimeOfDay;
            var minute = (float) time.TotalMinutes;
            var degree = minute * 6.0f;

            canvas.DrawLine(Color, Width, canvas.Point(degree, 0.05f), canvas.Point(degree, 0.70f));
        }
    }
}