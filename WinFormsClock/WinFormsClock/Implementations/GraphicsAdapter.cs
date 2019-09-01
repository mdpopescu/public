using System;
using System.Drawing;
using System.Linq;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    public class GraphicsAdapter : IExtendedGraphics
    {
        public GraphicsAdapter(IGraphicObjects graphicObjects, Graphics g)
        {
            this.graphicObjects = graphicObjects;
            this.g = g;
        }

        public void Clear(Color color)
        {
            g.Clear(color);
        }

        public void FillEllipse(Color color, RectangleF rect)
        {
            var brush = GetBrush(color);
            g.FillEllipse(brush, rect);
        }

        public void DrawLine(Color color, float width, PointF p1, PointF p2)
        {
            var pen = GetPen(color, width);
            g.DrawLine(pen, p1, p2);
        }

        public void DrawString(Color color, RectangleF rect, string text)
        {
            var brush = GetBrush(color);
            var font = GetAdjustedFont("XX", rect.Width, 36, 5);
            var format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            g.DrawString(text, font, brush, rect, format);
        }

        //

        private const string FONT_NAME = "Arial";

        private readonly IGraphicObjects graphicObjects;
        private readonly Graphics g;

        private static Func<Brush> BrushConstructor(Color color) => () => new SolidBrush(color);
        private static Func<Pen> PenConstructor(Color color, float width) => () => new Pen(color, width);
        private static Func<Font> FontConstructor(string fontName, int emSize) => () => new Font(fontName, emSize);

        private Brush GetBrush(Color color) => graphicObjects.Brushes.Get(color, BrushConstructor(color));
        private Pen GetPen(Color color, float width) => graphicObjects.Pens.Get(Tuple.Create(color, width), PenConstructor(color, width));
        private Font GetFont(int emSize) => graphicObjects.Fonts.Get(emSize, FontConstructor(FONT_NAME, emSize));

        private Func<Font, bool> TextFits(float width, string text) => font => g.MeasureString(text, font).Width <= width;

        private Font GetAdjustedFont(string text, float width, int maxSize, int minSize)
        {
            var candidates = Enumerable
                .Range(minSize, maxSize - minSize + 1)
                .Reverse()
                .Select(GetFont)
                .ToArray();

            // return either the first matching or the last (which is minSize)
            return candidates.Where(TextFits(width, text)).FirstOrDefault() ?? candidates.Last();
        }
    }
}