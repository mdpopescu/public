using System;
using System.Linq;

namespace TicTacToeAI.Implementations
{
    public class Neuron
    {
        public Func<float, float> Normalize { get; set; }

        public int N { get; }
        public float[] Inputs { get; } // range 0..1
        public float[] Weights { get; } // range 0..1

        public float Output => Normalize(Inputs.Zip(Weights, (i, w) => i * w).Sum());

        public Neuron(int n)
        {
            N = n;

            Inputs = new float[n];
            Weights = new float[n];

            Normalize = v => (float) (1.0 / (1.0 + Math.Exp(v)));
        }
    }
}