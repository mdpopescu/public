using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inventory2.Library
{
    /// <summary>
    /// Write-once, read-many stream; limited to 2GB so I can use ints
    /// </summary>
    public class WORMStream
    {
        public bool AutoFlush { get; set; } = true;

        public WORMStream(Stream stream)
        {
            this.stream = stream;
        }

        public void Append(byte[] buffer)
        {
            stream.Seek(0, SeekOrigin.End);
            stream.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
            stream.Write(buffer, 0, buffer.Length);
            if (AutoFlush)
                stream.Flush();
        }

        public void Flush()
        {
            stream.Flush();
        }

        public IEnumerable<byte[]> ReadAll()
        {
            var lengthBuffer = new byte[sizeof(int)];

            stream.Seek(0, SeekOrigin.Begin);
            while (Position < Length)
            {
                stream.Read(lengthBuffer, 0, sizeof(int));
                var buffer = new byte[BitConverter.ToInt32(lengthBuffer, 0)];
                stream.Read(buffer, 0, buffer.Length);
                yield return buffer;
            }
        }

        // helper method
        public void Append(string buffer)
        {
            Append(Encoding.UTF8.GetBytes(buffer));
        }

        //

        private readonly Stream stream;

        private int Position => (int) stream.Position;
        private int Length => (int) stream.Length;
    }
}