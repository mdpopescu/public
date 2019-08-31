using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    public class Canvas : ICanvas
    {
        public Canvas(Graphics g, PointF origin, int size)
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
            using (var brush = new SolidBrush(color))
                g.FillEllipse(brush, rect);
        }

        public void Line(Color color, float width, PointF startAt, PointF endAt)
        {
            using (var pen = new Pen(color, width))
                g.DrawLine(pen, startAt, endAt);
        }

        public void Text(Color color, RectangleF rect, string text)
        {
            using (var brush = new SolidBrush(color))
            using (var tempFont = new Font("Arial", 36))
            {
                var font = GetAdjustedFont("XX", tempFont, rect.Width, 36, 5);
                var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                g.DrawString(text, font, brush, rect, format);
            }
        }

        //

        private readonly Graphics g;
        private readonly PointF origin;
        private readonly int size;

        private Font GetAdjustedFont(string text, Font font, float width, int maxSize, int minSize)
        {
            var goodSize = Enumerable
                .Range(minSize, maxSize - minSize + 1)
                .Reverse()
                .Where(emSize => GetTextSize(text, font, emSize) <= width)
                .FirstOrDefault();
            if (goodSize < minSize)
                goodSize = minSize;
            return new Font(font.Name, goodSize, font.Style);
        }

        private float GetTextSize(string text, Font font, int emSize)
        {
            using (var testFont = new Font(font.Name, emSize, font.Style))
                return g.MeasureString(text, testFont).Width;
        }
    }
}