using System.Drawing;

namespace WinFormsClock.Contracts
{
    public interface ICanvas
    {
        PointF Origin { get; }
        int Size { get; }

        PointF Point(float degree, float radius);
        RectangleF Square(PointF center, float side);

        void Clear(Color color);

        void FillEllipse(Color color, RectangleF rect);
        void Line(Color color, float width, PointF startAt, PointF endAt);

        void Text(Color color, RectangleF rect, string text);
    }
}