using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToeAI.Contracts;

namespace TicTacToeAI.Extensions
{
    public class TicTacToeGame : Game
    {
        public float? Score
        {
            get
            {
                // the computer plays X -- that is, 1
                var complete = SPECIAL
                    .Where(coords => coords.All(it => it == coords[0]))
                    .FirstOrDefault();
                return complete?[0];
            }
        }

        public TicTacToeGame(Func<int[], int> getUserMove)
        {
            for (var i = 0; i < 9; i++)
                state[i] = 0.5f;

            this.getUserMove = getUserMove;
        }

        public bool HasEnded() => IsVictory() || !GetAvailable().Any();

        public float[] GetState() => state;

        public void Update(float[] values)
        {
            var available = GetAvailable().ToList();
            var move = available
                .Select(index => new { index, value = values[index] })
                .OrderByDescending(it => it.value)
                .Select(it => it.index)
                .First();

            state[move] = 1;
            available.Remove(move);

            if (HasEnded())
                return;

            var userMove = getUserMove(available.ToArray());
            state[userMove] = 0;
        }

        //

        private static readonly int[][] SPECIAL =
        {
            // rows
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6, 7, 8 },
            // cols
            new[] { 0, 3, 6 },
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },
            // diags
            new[] { 0, 4, 8 },
            new[] { 2, 4, 6 },
        };

        private readonly float[] state = new float[9]; // O = 0, X = 1, empty = 0.5

        private readonly Func<int[], int> getUserMove;

        private bool IsVictory() => SPECIAL.Any(state.HaveSameValue);

        private IEnumerable<int> GetAvailable()
        {
            return state
                .Select((v, index) => new { v, index })
                .Where(it => it.v.IsEqualTo(0.5f))
                .Select(it => it.index);
        }
    }
}