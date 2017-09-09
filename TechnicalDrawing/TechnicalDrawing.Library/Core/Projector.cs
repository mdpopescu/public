using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Core
{
    public class Projector
    {
        public Point2D[] Project(Plane plane, params float[] values)
        {
            var projection = PROJECTION_INDICES[(int) plane];

            return values
                .Select((value, index) => new { value, index })
                .GroupBy(tuple => tuple.index / 3)
                .Select(g => g.Select(it => it.value).ToArray())
                .Select(arr => MapToPlane(arr, projection))
                .ToArray();
        }

        //

        private static readonly List<Tuple<int, int>> PROJECTION_INDICES = new List<Tuple<int, int>>
        {
            Tuple.Create(0, 1),
            Tuple.Create(0, 2),
            Tuple.Create(1, 2),
        };

        /// <summary>Maps 3D coordinates to a plane.</summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="projection">The appropriate indices for the given plane.</param>
        /// <returns>The mapped coordinates (rounded to the nearest integer).</returns>
        private static Point2D MapToPlane(IReadOnlyList<float> coordinates, Tuple<int, int> projection) =>
            new Point2D((int) Math.Round(coordinates[projection.Item1]), (int) Math.Round(coordinates[projection.Item2]));
    }
}