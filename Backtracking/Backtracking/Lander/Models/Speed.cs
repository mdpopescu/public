namespace Backtracking.Lander.Models
{
    public class Speed
    {
        public double HSpeed { get; }
        public double VSpeed { get; }

        public Speed(double hSpeed, double vSpeed)
        {
            HSpeed = hSpeed;
            VSpeed = vSpeed;
        }

        public override string ToString() => $"dH={HSpeed} dV={VSpeed}";

        public Speed Add(Speed other) => new Speed(HSpeed + other.HSpeed, VSpeed + other.VSpeed);

        public Speed Multiply(double factor) => new Speed(HSpeed * factor, VSpeed * factor);
    }
}