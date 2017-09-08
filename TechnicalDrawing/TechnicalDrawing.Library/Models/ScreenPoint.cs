namespace TechnicalDrawing.Library.Models
{
    public struct ScreenPoint
    {
        public ScreenPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}