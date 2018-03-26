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
    }
}