using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SecurePasswordStorage.Library.Models
{
    public class ByteArrayTuple : IEquatable<ByteArrayTuple>
    {
        public static ByteArrayTuple Create(IEnumerable<byte> item1, IEnumerable<byte> item2) =>
            new ByteArrayTuple(item1, item2);

        public static ByteArrayTuple FromArray(byte[] bytes)
        {
            var prefix = bytes.Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(prefix);
            var count = BitConverter.ToInt32(prefix);
            var item1 = bytes.Skip(4).Take(count);
            var item2 = bytes.Skip(4 + count);
            return Create(item1, item2);
        }

        //

        public IEnumerable<byte> Item1 { get; }
        public IEnumerable<byte> Item2 { get; }

        public ByteArrayTuple(IEnumerable<byte> item1, IEnumerable<byte> item2)
        {
            Item1 = item1.ToArray();
            Item2 = item2.ToArray();
        }

        public void Deconstruct(out byte[] salt, out byte[] hash)
        {
            salt = Item1.ToArray();
            hash = Item2.ToArray();
        }

        public byte[] ToArray()
        {
            var prefix = BitConverter.GetBytes(Item1.Count());
            if (BitConverter.IsLittleEndian)
                Array.Reverse(prefix);

            return prefix
                .Concat(Item1)
                .Concat(Item2)
                .ToArray();
        }

        #region IEquatable

        public bool Equals(ByteArrayTuple other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Item1.SequenceEqual(other.Item1) && Item2.SequenceEqual(other.Item2);
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ByteArrayTuple) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Item1, Item2);

        public static bool operator ==(ByteArrayTuple left, ByteArrayTuple right) => Equals(left, right);

        public static bool operator !=(ByteArrayTuple left, ByteArrayTuple right) => !Equals(left, right);

        #endregion
    }
}