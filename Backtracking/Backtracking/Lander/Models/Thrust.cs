namespace Backtracking.Lander.Models
{
    public class Thrust
    {
        public int Angle { get; }
        public int Power { get; }

        public Thrust(int angle, int power)
        {
            Angle = angle;
            Power = power;
        }

        public override string ToString() => $"{Angle} {Power}";

        public override bool Equals(object obj) => obj is Thrust other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = -253139362;
                hashCode = hashCode * -1521134295 + Angle.GetHashCode();
                hashCode = hashCode * -1521134295 + Power.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(Thrust other) => Angle == other.Angle && Power == other.Power;

        public static bool operator ==(Thrust left, Thrust right) => Equals(left, right);
        public static bool operator !=(Thrust left, Thrust right) => !Equals(left, right);
    }
}