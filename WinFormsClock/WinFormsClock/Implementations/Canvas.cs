using System;
using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    public class Canvas : ICanvas, IDisposable
    {
        public Canvas(Graphics g, PointF origin, int size)
        {
            this.g = g;
            this.origin = origin;
            this.size = size;
        }

        public void Dispose()
        {
            brushes.Dispose();
            pens.Dispose();
            fonts.Dispose();
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
            var brush = brushes.Get(color, () => new SolidBrush(color));
            g.FillEllipse(brush, rect);
        }

        public void Line(Color color, float width, PointF startAt, PointF endAt)
        {
            var pen = pens.Get(Tuple.Create(color, width), () => new Pen(color, width));
            g.DrawLine(pen, startAt, endAt);
        }

        public void Text(Color color, RectangleF rect, string text)
        {
            var brush = brushes.Get(color, () => new SolidBrush(color));
            var tempFont = fonts.Get(36, () => new Font(FONT_NAME, 36));
            var font = GetAdjustedFont("XX", tempFont, rect.Width, 36, 5);
            var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            g.DrawString(text, font, brush, rect, format);
        }

        //

        private const string FONT_NAME = "Arial";

        private readonly ICache<Brush, Color> brushes = new Cache<Brush, Color>(10);
        private readonly ICache<Pen, Tuple<Color, float>> pens = new Cache<Pen, Tuple<Color, float>>(10);
        private readonly ICache<Font, int> fonts = new Cache<Font, int>(50);

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
            var testFont = fonts.Get(emSize, () => new Font(font.Name, emSize, font.Style));
            return g.MeasureString(text, testFont).Width;
        }
    }
}