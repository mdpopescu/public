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

        public override bool Equals(object obj) => obj is Position other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 1861411795;
                hashCode = hashCode * -1521134295 + X.GetHashCode();
                hashCode = hashCode * -1521134295 + Y.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(Position other) => X == other.X && Y == other.Y;

        public static bool operator ==(Position left, Position right) => Equals(left, right);
        public static bool operator !=(Position left, Position right) => !Equals(left, right);
    }
}