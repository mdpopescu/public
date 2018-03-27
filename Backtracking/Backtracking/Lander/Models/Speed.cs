using System;

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

        public override bool Equals(object obj) => obj is Speed other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = -1960253210;
                hashCode = hashCode * -1521134295 + HSpeed.GetHashCode();
                hashCode = hashCode * -1521134295 + VSpeed.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(Speed other) => Math.Abs(HSpeed - other.HSpeed) < 0.001 && Math.Abs(VSpeed - other.VSpeed) < 0.001;

        public static bool operator ==(Speed left, Speed right) => Equals(left, right);
        public static bool operator !=(Speed left, Speed right) => !Equals(left, right);

        public Speed Add(Speed other) => new Speed(HSpeed + other.HSpeed, VSpeed + other.VSpeed);

        public Speed Multiply(double factor) => new Speed(HSpeed * factor, VSpeed * factor);
    }
}