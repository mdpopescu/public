using System;
using CustomCrypto.Library.Contracts;

namespace CustomCrypto.Library.Implementations
{
    public class PredictableRNG : RNG
    {
        public PredictableRNG(int seed)
        {
            rng = new Random(seed);
        }

        public uint Next()
        {
            return (uint) rng.Next();
        }

        //

        private readonly Random rng;
    }
}