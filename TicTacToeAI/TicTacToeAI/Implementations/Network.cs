using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TicTacToeAI.Implementations
{
    public class Network
    {
        public Network(int inputCount, params int[] layerCount)
        {
            this.inputCount = inputCount;

            neurons = new Neuron[layerCount.Length][];
            for (var i = 0; i < layerCount.Length; i++)
                neurons[i] = CreateLayer(layerCount[i], i > 0 ? layerCount[i - 1] : inputCount);
        }

        public void Randomize()
        {
            ForEachLayer(
                layer =>
                    ForEachNeuron(
                        layer,
                        neuron =>
                        {
                            for (var i = 0; i < neuron.Weights.Length; i++)
                                neuron.Weights[i] = (float) RND.NextDouble();
                        }));
        }

        public float[] Compute(params float[] inputs)
        {
            Debug.Assert(inputs.Length == inputCount);

            var state = inputs;
            ForEachLayer(
                layer =>
                {
                    SetInputs(layer, state);
                    state = ComputeOutputs(layer);
                });

            return state;
        }

        public Network[] Mix(Network other, int offspringCount)
        {
            // assumes the networks have the same size
            var result = new Network(inputCount, neurons.Select(layer => layer.Length).ToArray());

            // average the two networks
            for (var i = 0; i < neurons.Length; i++)
            for (var j = 0; j < neurons[i].Length; j++)
            {
                var neuron = result.neurons[i][j];
                for (var k = 0; k < neuron.Weights.Length; k++)
                    neuron.Weights[k] = Average(neurons[i][j].Weights[k], other.neurons[i][j].Weights[k]);
            }

            return Enumerable
                .Range(1, offspringCount)
                .Select(_ => CreateVariation(result))
                .ToArray();
        }

        //

        private static readonly Random RND = new Random();

        private readonly int inputCount;
        private readonly Neuron[][] neurons;

        private static float Average(float a, float b) => (a + b) / 2f;

        private static Neuron[] CreateLayer(int neuronCount, int inputCount)
        {
            var layer = new Neuron[neuronCount];
            for (var i = 0; i < neuronCount; i++)
                layer[i] = new Neuron(inputCount);
            return layer;
        }

        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        private static void SetInputs(IReadOnlyList<Neuron> layer, IReadOnlyList<float> inputs)
        {
            for (var i = 0; i < inputs.Count; i++)
                ForEachNeuron(layer, neuron => neuron.Inputs[i] = inputs[i]);
        }

        private static float[] ComputeOutputs(IEnumerable<Neuron> layer)
        {
            return layer.Select(neuron => neuron.Output).ToArray();
        }

        private Network CreateVariation(Network model)
        {
            const float VARIATION = 0.01f;

            throw new NotImplementedException();
        }

        private void ForEachLayer(Action<Neuron[]> action)
        {
            foreach (var layer in neurons)
                action(layer);
        }

        private static void ForEachNeuron(IEnumerable<Neuron> layer, Action<Neuron> action)
        {
            foreach (var neuron in layer)
                action(neuron);
        }
    }
}