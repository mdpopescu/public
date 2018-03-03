using System;
using System.Diagnostics;
using System.Linq;
using TicTacToeAI.Contracts;

namespace TicTacToeAI.Implementations
{
    public class Neuron : Cloneable<Neuron>
    {
        public Func<float, float> Normalize { get; set; }

        public int N { get; }
        public float[] Weights { get; } // range 0..1

        public float Compute(float[] inputs)
        {
            Debug.Assert(inputs.Length == N);
            return Normalize(inputs.Zip(Weights, (i, w) => i * w).Sum());
        }

        public Neuron(int n)
        {
            N = n;

            Weights = new float[n];

            Normalize = v => (float) (1.0 / (1.0 + Math.Exp(v)));
        }

        public Neuron Clone()
        {
            var result = new Neuron(N) { Normalize = Normalize };
            for (var i = 0; i < N; i++)
            {
                result.Weights[i] = Weights[i];
            }

            return result;
        }
    }
}