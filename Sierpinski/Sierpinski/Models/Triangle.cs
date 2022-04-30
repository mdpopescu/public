using System.Diagnostics;
using System.Drawing;

namespace Sierpinski.Models
{
    public class Triangle
    {
        // the upper vertex comes first, then the leftmost vertex
        public Point[] Points { get; } =
        {
            new(Extensions.MAX_X / 2, PADDING),
            new(PADDING, Extensions.MAX_Y - PADDING),
            new(Extensions.MAX_X - PADDING, Extensions.MAX_Y - PADDING),
        };

        public Triangle()
        {
            leftSlope = (float)(Points[1].X - Points[0].X) / (Points[1].Y - Points[0].Y);
            Debug.Assert(leftSlope < 0);
            rightSlope = (float)(Points[2].X - Points[0].X) / (Points[2].Y - Points[0].Y);
            Debug.Assert(rightSlope > 0);
        }

        public bool IsInside(Point p) =>
            p.Y is >= Y_MIN and <= Y_MAX && IsXInside(p);

        public Point GenerateRandomPointInside()
        {
            var p = GenerateRandomPoint();
            while (!IsInside(p))
                p = GenerateRandomPoint();

            return p;
        }

        public Point PickRandomVertex() =>
            Points.PickRandom();

        //

        private const int PADDING = 5;

        private const int X_MIN = PADDING;
        private const int X_MAX = Extensions.MAX_X - PADDING;
        private const int Y_MIN = PADDING;
        private const int Y_MAX = Extensions.MAX_Y - PADDING;

        private readonly float leftSlope, rightSlope;

        private bool IsXInside(Point p) =>
            p.X > (p.Y - Y_MIN) * leftSlope && p.X < (p.Y - Y_MIN) * rightSlope;

        private static Point GenerateRandomPoint() =>
            new(Extensions.RND.Next(X_MIN, X_MAX), Extensions.RND.Next(Y_MIN, Y_MAX));
    }
}