using System.Drawing;

namespace WinFormsClock.Contracts
{
    public interface IExtendedGraphics
    {
        void Clear(Color color);

        void FillEllipse(Color color, RectangleF rect);
        void DrawLine(Color color, float width, PointF p1, PointF p2);
        void DrawString(Color color, RectangleF rect, string text);
    }
}