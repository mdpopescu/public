using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TicTacToeAI.Extensions
{
    public static class Extensions
    {
        public static bool IsEqualTo(this float a, float b) => Math.Abs(a - b) < 0.001f;

        public static bool AllEqualTo(this float[] values, int[] coords, float value)
        {
            Contract.Requires(coords != null);
            Contract.Requires(coords.Length > 0);
            Contract.Requires(values != null);
            Contract.Requires(values.Length >= coords.Length);

            return coords.All(coord => values[coord].IsEqualTo(value));
        }

        public static bool AllEqualTo(this int[] coords, float[] values, float value)
        {
            Contract.Requires(coords != null);
            Contract.Requires(coords.Length > 0);
            Contract.Requires(values != null);
            Contract.Requires(values.Length >= coords.Length);

            return coords.All(coord => values[coord].IsEqualTo(value));
        }
    }
}