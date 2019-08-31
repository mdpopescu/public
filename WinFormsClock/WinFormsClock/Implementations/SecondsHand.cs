using System;
using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class SecondsHand : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;
        public float Width { get; set; } = 1.0f;

        public void Draw(ICanvas canvas)
        {
            var time = DateTime.Now.TimeOfDay;
            var second = (float) time.TotalSeconds;
            var degree = second * 6.0f;

            canvas.Line(Color, Width, canvas.Point(degree, 0.05f), canvas.Point(degree, 0.85f));
        }
    }
}