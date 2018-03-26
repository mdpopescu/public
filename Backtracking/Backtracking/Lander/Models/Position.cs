namespace Backtracking.Lander.Models
{
    public class Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"X={X} Y={Y}";
    }
}