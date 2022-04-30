using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Sierpinski.Models
{
    public class Triangle
    {
        // the upper vertex comes first, then the leftmost vertex
        public Point[] Points { get; } =
        {
            new(49, 5),
            new(5, 95),
            new(95, 95),
        };

        public Triangle()
        {
            xMin = Points.Min(it => it.X) + PADDING;
            xMax = Points.Max(it => it.X) - PADDING;
            yMin = Points.Min(it => it.Y) + PADDING;
            yMax = Points.Max(it => it.Y) - PADDING;

            leftSlope = (float)(Points[1].X - Points[0].X) / (Points[1].Y - Points[0].Y);
            Debug.Assert(leftSlope < 0);
            rightSlope = (float)(Points[2].X - Points[0].X) / (Points[2].Y - Points[0].Y);
            Debug.Assert(rightSlope > 0);
        }

        public bool IsInside(Point p) =>
            p.Y >= yMin && p.Y <= yMax && IsXInside(p);

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

        private readonly int xMin, xMax, yMin, yMax;
        private readonly float leftSlope, rightSlope;

        private bool IsXInside(Point p) =>
            p.X > (p.Y - yMin) * leftSlope + PADDING && p.X < (p.Y - yMin) * rightSlope - PADDING;

        private Point GenerateRandomPoint() =>
            new(Extensions.RND.Next(xMin, xMax), Extensions.RND.Next(yMin, yMax));
    }
}