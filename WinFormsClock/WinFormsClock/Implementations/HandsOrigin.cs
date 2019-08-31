using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class HandsOrigin : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;

        public void Draw(ICanvas canvas)
        {
            canvas.FillEllipse(Color, canvas.Square(canvas.Point(0.0f, 0.0f), 0.1f));
        }
    }
}