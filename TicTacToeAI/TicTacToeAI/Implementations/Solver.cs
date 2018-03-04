using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToeAI.Contracts;
using TicTacToeAI.Models;

namespace TicTacToeAI.Implementations
{
    public class Solver
    {
        public Action<int> OnGeneration { get; set; } = _ => { };

        public Solver(Func<Game> gameFactory)
        {
            this.gameFactory = gameFactory;
        }

        public Network Solve(Network network, int generationCount)
        {
            return Enumerable
                .Range(1, generationCount)
                .Aggregate(network.Mix(network, OFFSPRING_COUNT), Next)
                .First();
        }

        //

        private readonly Func<Game> gameFactory;

        private const int OFFSPRING_COUNT = 10;
        private const int BEST_COUNT = 3;

        private Network[] Next(Network[] generation, int index)
        {
            OnGeneration.Invoke(index);

            var next = generation
                .SelectMany(a => generation, (a, b) => a.Mix(b, OFFSPRING_COUNT))
                .SelectMany(it => it)
                .ToArray();
            return GetBest(next).ToArray();
        }

        private IEnumerable<Network> GetBest(IEnumerable<Network> generation)
        {
            return generation
                .Select(candidate => new { candidate, score = EvalCandidate(candidate) })
                .OrderByDescending(it => it.score)
                .Select(it => it.candidate)
                .Take(BEST_COUNT);
        }

        private float EvalCandidate(Network candidate)
        {
            var game = gameFactory.Invoke();

            do
            {
                var outputs = candidate.Compute(game.GetState());
                var move = new TicTacToeMove { Index = (int) outputs[0], Value = 1 };

                // if the NN attempts an illegal move it loses immediately
                if (!game.TryMove(move))
                    return 0.0f;

                // TODO: get an opponent
                // TODO: get the opponent to move
                // TODO: if the opponent makes an illegal move the candidate wins immediately
            }
            while (!game.HasEnded());

            return game.Score.GetValueOrDefault();
        }
    }
}