using System;
using System.Drawing;

namespace WinFormsClock.Models
{
    public class PolarCoord
    {
        public static PolarCoord Create(float degree, float radius) => new PolarCoord(degree, radius);

        //

        public float Degree { get; }
        public float Radius { get; }

        public PointF CarthesianCoord { get; }

        //

        private const float DEGTORAD_FACTOR = (float) (Math.PI / 180);

        private static float DegToRad(float degree) => DEGTORAD_FACTOR * degree;

        private PolarCoord(float degree, float radius)
        {
            // rotate the coordinates so that 0 is straight up
            degree = (degree + 300) % 360;

            Degree = degree;
            Radius = radius;

            var x = (float) Math.Cos(DegToRad(degree)) * radius;
            var y = (float) Math.Sin(DegToRad(degree)) * radius;
            CarthesianCoord = new PointF(x, y);
        }
    }
}