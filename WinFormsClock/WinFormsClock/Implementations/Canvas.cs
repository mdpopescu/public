using System;
using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;
using WinFormsClock.Helpers;
using WinFormsClock.Models;

namespace WinFormsClock.Implementations
{
    public class Canvas : ICanvas
    {
        public Canvas(IGraphicObjects graphicObjects, Graphics g, PointF origin, int size)
        {
            this.graphicObjects = graphicObjects;
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
            var brush = graphicObjects.Brushes.Get(color, BrushConstructor(color));
            g.FillEllipse(brush, rect);
        }

        public void Line(Color color, float width, PointF startAt, PointF endAt)
        {
            var pen = graphicObjects.Pens.Get(Tuple.Create(color, width), PenConstructor(color, width));
            g.DrawLine(pen, startAt, endAt);
        }

        public void Text(Color color, RectangleF rect, string text)
        {
            var brush = graphicObjects.Brushes.Get(color, BrushConstructor(color));
            var font = GetFont(rect.Width);
            var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            g.DrawString(text, font, brush, rect, format);
        }

        //

        private const string FONT_NAME = "Arial";

        private readonly IGraphicObjects graphicObjects;
        private readonly Graphics g;
        private readonly PointF origin;
        private readonly int size;

        private static Func<Brush> BrushConstructor(Color color) => () => new SolidBrush(color);
        private static Func<Pen> PenConstructor(Color color, float width) => () => new Pen(color, width);
        private static Func<Font> FontConstructor(Font font, int emSize) => () => new Font(font.Name, emSize, font.Style);

        private Func<int, Font> GetFontAtSize(Font font) => emSize => graphicObjects.Fonts.Get(emSize, FontConstructor(font, emSize));
        private Func<Font, bool> TextFits(float width, string text) => font => g.MeasureString(text, font).Width <= width;

        private Font GetFont(float width)
        {
            var tempFont = graphicObjects.Fonts.Get(36, () => new Font(FONT_NAME, 36));
            return GetAdjustedFont("XX", tempFont, width, 36, 5);
        }

        private Font GetAdjustedFont(string text, Font font, float width, int maxSize, int minSize)
        {
            var candidates = Enumerable
                .Range(minSize, maxSize - minSize + 1)
                .Reverse()
                .Select(GetFontAtSize(font))
                .ToArray();

            // return either the first matching or the last (which is minSize)
            return candidates.Where(TextFits(width, text)).FirstOrDefault() ?? candidates.Last();
        }
    }
}