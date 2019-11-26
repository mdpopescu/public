using System;
using System.Linq;

namespace Zaruri
{
    internal class Indices
    {
        public static Indices Create(Index[] values) => new Indices(values);

        //

        public Index[] Values { get; }

        public Indices(Index[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length > Constants.DICE_COUNT)
                throw new ArgumentException("You must enter at most 5 indices.");
            if (values.Distinct().Count() != values.Length)
                throw new ArgumentException("You must enter distinct indices.");

            Values = values;
        }

        public bool Contains(int index) => Values.Select(it => it.Value).Contains(index);
    }
}