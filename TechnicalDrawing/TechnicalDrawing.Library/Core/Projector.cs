using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Core
{
    public class Projector
    {
        public QuadrantPoint[] Project(Plane plane, params float[] values)
        {
            var projection = PROJECTION_INDICES[plane];

            return values
                .Select((value, index) => new { value, index })
                .GroupBy(tuple => tuple.index / 3)
                .Select(g => g.Select(it => it.value).ToArray())
                .Select(arr => MapToPlane(arr, projection))
                .ToArray();
        }

        //

        private static readonly Dictionary<Plane, Tuple<int, int>> PROJECTION_INDICES = new Dictionary<Plane, Tuple<int, int>>
        {
            { Plane.XY, Tuple.Create(0, 1) },
            { Plane.XZ, Tuple.Create(0, 2) },
            { Plane.YZ, Tuple.Create(1, 2) },
        };

        /// <summary>Maps 3D coordinates to a plane.</summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="projection">The appropriate indices for the given plane.</param>
        /// <returns>The mapped coordinates (rounded to the nearest integer).</returns>
        private static QuadrantPoint MapToPlane(IReadOnlyList<float> coordinates, Tuple<int, int> projection) =>
            new QuadrantPoint((int) Math.Round(coordinates[projection.Item1]), (int) Math.Round(coordinates[projection.Item2]));
    }
}