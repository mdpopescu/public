using System.Drawing;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    public class Canvas : ICanvas
    {
        public Canvas(IExtendedGraphics g, PointF origin, int size)
        {
            this.g = g;
            this.origin = origin;
            this.size = size;
        }

        public PointF Point(float degree, float radius)
        {
            return PolarCoord.Create(degree, size * radius).CarthesianCoord.Offset(origin);
        }

        public RectangleF Square(PointF center, float side)
        {
            var delta = size * side;
            return new RectangleF(center.X - delta / 2, center.Y - delta / 2, delta, delta);
        }

        public void Clear(Color color)
        {
            g.Clear(color);
        }

        public void FillEllipse(Color color, RectangleF rect)
        {
            g.FillEllipse(color, rect);
        }

        public void DrawLine(Color color, float width, PointF p1, PointF p2)
        {
            g.DrawLine(color, width, p1, p2);
        }

        public void DrawString(Color color, RectangleF rect, string text)
        {
            g.DrawString(color, rect, text);
        }

        //

        private readonly IExtendedGraphics g;
        private readonly PointF origin;
        private readonly int size;
    }
}