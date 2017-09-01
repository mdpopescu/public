using System;
using System.IO;
using Renfield.AppendOnly.Library.Contracts;

namespace Renfield.AppendOnly.Library.Services
{
    public class StreamAccessor : RandomAccessor
    {
        public StreamAccessor(Stream stream)
        {
            this.stream = stream;

            length = stream.Length;
        }

        public long get_length()
        {
            lock (lockObject)
                return length;
        }

        public int read_int(long position)
        {
            var buffer = read_bytes(position, sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public byte[] read_bytes(long position, int size)
        {
            var buffer = new byte[size];

            lock (lockObject)
            {
                stream.Seek(position, SeekOrigin.Begin);
                stream.Read(buffer, 0, size);
            }

            return buffer;
        }

        public void write_int(long position, int value)
        {
            write_bytes(position, BitConverter.GetBytes(value));
        }

        public void write_bytes(long position, byte[] value)
        {
            var len = value.Length;

            lock (lockObject)
            {
                stream.Seek(position, SeekOrigin.Begin);
                stream.Write(value, 0, len);

                if (length < position + len)
                    length = position + len;
            }
        }

        //

        private readonly object lockObject = new object();

        private readonly Stream stream;

        private long length;
    }
}