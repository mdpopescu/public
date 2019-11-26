using System;

namespace Zaruri
{
    internal class Index
    {
        public static Index Create(int value) => new Index(value);

        //

        public int Value { get; }

        public Index(int value)
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("The value must be between 1 and 5.");

            Value = value;
        }
    }
}