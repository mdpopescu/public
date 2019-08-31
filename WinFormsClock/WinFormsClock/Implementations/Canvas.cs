using System.Drawing;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    public class Canvas : ICanvas
    {
        public PointF Origin { get; }
        public int Size { get; }

        public Canvas(Graphics g, PointF origin, int size)
        {
            this.g = g;
            Origin = origin;
            Size = size;
        }

        public PointF Point(float degree, float radius)
        {
            return PolarCoord.Create(degree, Size * radius).CarthesianCoord.Offset(Origin);
        }

        public RectangleF Square(PointF center, float side)
        {
            var delta = Size * side;
            return new RectangleF(center.X - delta / 2, center.Y - delta / 2, delta, delta);
        }

        public void Clear(Color color)
        {
            g.Clear(color);
        }

        public void FillEllipse(Color color, RectangleF rect)
        {
            using (var brush = new SolidBrush(color))
                g.FillEllipse(brush, rect);
        }

        public void Line(Color color, float width, PointF startAt, PointF endAt)
        {
            using (var pen = new Pen(color, width))
                g.DrawLine(pen, startAt, endAt);
        }

        //

        private readonly Graphics g;
    }
}