using System;
using System.Collections.Generic;

namespace Renfield.AppendOnly.Library
{
    public class LowLevelAppendOnlyFile : LowLevelAppendOnly
    {
        public long[] GetIndex() => index.ToArray();

        public LowLevelAppendOnlyFile(RandomAccessor data, IEnumerable<long> index = null)
        {
            this.data = data;
            this.index = new List<long>(index ?? RebuildIndex());
        }

        /// <summary>
        /// Adds a record to the file
        /// </summary>
        /// <param name="record">Record to add</param>
        public void Append(byte[] record)
        {
            lock (lockObject)
            {
                var position = data.get_length();
                data.write_long(position, record.Length);
                index.Add(position);

                position += sizeof(long);
                data.write_bytes(position, record);
            }
        }

        /// <summary>
        /// Returns the i-th record
        /// </summary>
        /// <param name="i">The record number (0-based)</param>
        /// <returns>The contents of the record</returns>
        public byte[] Read(int i)
        {
            var length = data.get_length();
            var position = index[i];

            var size = data.read_long(position);
            position += sizeof(long);

            if (position + size > length)
                throw new Exception($"Internal error: cannot read {size} bytes starting at {position}; i = {i}, Index[i] = {index[i]}, length = {length}");

            return data.read_bytes(position, size);
        }

        /// <summary>
        /// Returns all records from the i-th on
        /// </summary>
        /// <param name="i">The record number (0-based) to start reading from</param>
        /// <returns>The records from the i-th on</returns>
        public IEnumerable<byte[]> ReadFrom(int i)
        {
            var length = data.get_length();
            var position = index[i];
            while (position < length)
            {
                var size = data.read_long(position);
                position += sizeof(long);

                yield return data.read_bytes(position, size);
                position += size;
            }
        }

        //

        private readonly RandomAccessor data;
        private readonly List<long> index;
        private readonly object lockObject = new object();

        private IEnumerable<long> RebuildIndex()
        {
            var length = data.get_length();
            long position = 0;
            while (position < length)
            {
                yield return position;

                var size = data.read_long(position);
                position += size + sizeof(long);
            }
        }
    }
}