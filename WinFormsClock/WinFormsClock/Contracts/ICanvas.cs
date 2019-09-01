using System.Drawing;

namespace WinFormsClock.Contracts
{
    public interface ICanvas: IExtendedGraphics
    {
        PointF Point(float degree, float radius);
        RectangleF Square(PointF center, float side);
    }
}