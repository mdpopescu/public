using System.Drawing;

namespace WinFormsClock.Helpers
{
    public static class Extensions
    {
        public static PointF Offset(this PointF point, PointF delta) => new PointF(point.X + delta.X, point.Y + delta.Y);
    }
}