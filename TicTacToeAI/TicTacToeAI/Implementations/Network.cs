using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TicTacToeAI.Contracts;

namespace TicTacToeAI.Implementations
{
    public class Network : Cloneable<Network>
    {
        public Network(int inputCount, params int[] layerCount)
        {
            this.inputCount = inputCount;

            neurons = new Neuron[layerCount.Length][];
            for (var i = 0; i < layerCount.Length; i++)
                neurons[i] = CreateLayer(layerCount[i], i > 0 ? layerCount[i - 1] : inputCount);
        }

        public Network Clone()
        {
            var result = new Network(inputCount, NeuronCounts);

            for (var i = 0; i < LayerCount; i++)
            for (var j = 0; j < NeuronCounts[i]; j++)
                result.neurons[i][j] = neurons[i][j].Clone();

            return result;
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
            return neurons.Aggregate(inputs, (state, layer) => layer.Select(neuron => neuron.Compute(state)).ToArray());
        }

        public Network[] Mix(Network other, int offspringCount)
        {
            // assumes the networks have the same size
            var result = new Network(inputCount, neurons.Select(layer => layer.Length).ToArray());

            // average the two networks
            for (var i = 0; i < LayerCount; i++)
            for (var j = 0; j < NeuronCounts[i]; j++)
            {
                var neuron = result.neurons[i][j];
                for (var k = 0; k < neuron.Weights.Length; k++)
                    neuron.Weights[k] = Average(neurons[i][j].Weights[k], other.neurons[i][j].Weights[k]);
            }

            return Enumerable
                .Range(1, offspringCount)
                .Select(_ => result.CreateVariation())
                .ToArray();
        }

        //

        private static readonly Random RND = new Random();

        private readonly int inputCount;
        private readonly Neuron[][] neurons;

        private int LayerCount => neurons.Length;
        private int[] NeuronCounts => neurons.Select(layer => layer.Length).ToArray();

        private static float Average(float a, float b) => (a + b) / 2f;

        private static Neuron[] CreateLayer(int neuronCount, int inputCount)
        {
            var layer = new Neuron[neuronCount];
            for (var i = 0; i < neuronCount; i++)
                layer[i] = new Neuron(inputCount);
            return layer;
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

        private Network CreateVariation()
        {
            const float VARIATION = 0.01f;

            var clone = Clone();
            clone.ForEachLayer(
                layer =>
                    ForEachNeuron(
                        layer,
                        neuron =>
                        {
                            for (var i = 0; i < neuron.Weights.Length; i++)
                                neuron.Weights[i] += (float) (VARIATION * (RND.NextDouble() * 2 - 1));
                        }));
            return clone;
        }
    }
}