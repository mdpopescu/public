using System.Collections.Generic;
using System.Linq;

namespace SecurePasswordStorage.Library.Models
{
    public class ByteArrayTuple
    {
        public static ByteArrayTuple Create(IEnumerable<byte> item1, IEnumerable<byte> item2) =>
            new ByteArrayTuple(item1, item2);

        //

        public IEnumerable<byte> Item1 { get; }
        public IEnumerable<byte> Item2 { get; }

        public ByteArrayTuple(IEnumerable<byte> item1, IEnumerable<byte> item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public void Deconstruct(out byte[] salt, out byte[] hash)
        {
            salt = Item1.ToArray();
            hash = Item2.ToArray();
        }
    }
}