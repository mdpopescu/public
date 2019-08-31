using System.Drawing;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    internal class HandsOrigin : IClockPart
    {
        public Color Color { get; set; } = Color.BlueViolet;

        public void Draw(Graphics g, Point origin, int size)
        {
            using (var brush = new SolidBrush(Color))
            {
                var drawingSize = size * 0.05f;
                var drawingRect = new RectangleF(origin.X - drawingSize, origin.Y - drawingSize, drawingSize * 2, drawingSize * 2);

                g.FillEllipse(brush, drawingRect);
            }
        }

        public void Draw(ICanvas canvas)
        {
            canvas.FillEllipse(Color, canvas.Square(canvas.Point(0.0f, 0.0f), 0.1f));
        }
    }
}