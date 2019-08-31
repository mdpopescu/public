using System;
using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class HoursHand : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;
        public float Width { get; set; } = 1.0f;

        public void Draw(ICanvas canvas)
        {
            var time = DateTime.Now.TimeOfDay;
            var hour = (float) time.TotalHours % 12;
            var degree = hour * 30.0f;

            canvas.Line(Color, Width, canvas.Point(degree, 0.05f), canvas.Point(degree, 0.50f));
        }
    }
}