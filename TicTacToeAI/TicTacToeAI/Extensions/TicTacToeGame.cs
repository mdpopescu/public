using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToeAI.Contracts;
using TicTacToeAI.Models;

namespace TicTacToeAI.Extensions
{
    public class TicTacToeGame : Game
    {
        public float? Score
        {
            get
            {
                if (IsVictory())
                    return 1.0f;
                if (IsLoss())
                    return 0.0f;
                if (GetAvailable().Any())
                    return null;
                return 0.5f;
            }
        }

        public TicTacToeGame(float valueToPlay, Func<int[], int> getUserMove)
        {
            this.valueToPlay = valueToPlay;
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

        public bool TryMove(object move)
        {
            if (!(move is TicTacToeMove tttMove))
                return false;

            var available = GetAvailable().ToList();
            if (!available.Contains(tttMove.Index))
                return false;

            if (!new[] { 0, 1 }.Contains(tttMove.Value))
                return false;

            state[tttMove.Index] = tttMove.Value;
            return true;
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

        private readonly float[] state = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }; // O = 0, X = 1, empty = 0.5

        private readonly float valueToPlay;
        private readonly Func<int[], int> getUserMove;

        private bool IsVictory() => SPECIAL.Any(it => it.AllEqualTo(state, valueToPlay));
        private bool IsLoss() => SPECIAL.Any(it => it.AllEqualTo(state, 1.0f - valueToPlay));

        private IEnumerable<int> GetAvailable()
        {
            return state
                .Select((v, index) => new { v, index })
                .Where(it => it.v.IsEqualTo(0.5f))
                .Select(it => it.index);
        }
    }
}