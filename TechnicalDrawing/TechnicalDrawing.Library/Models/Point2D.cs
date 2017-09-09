namespace TechnicalDrawing.Library.Models
{
    public struct Point2D
    {
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString() => $"({X}, {Y})";
    }
}