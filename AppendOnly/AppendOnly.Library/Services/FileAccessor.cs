using System;
using Renfield.AppendOnly.Library.Contracts;

namespace Renfield.AppendOnly.Library.Services
{
    public class FileAccessor : RandomAccessor, IDisposable
    {
        public FileAccessor(string filename)
        {
            file = new WinFileIO();
            file.OpenForReadingWriting(filename);
        }

        public void Dispose()
        {
            file.Dispose();
        }

        public long get_length() => file.Length;

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
                file.Position = position;
                file.Read(buffer);
            }

            return buffer;
        }

        public void write_int(long position, int value)
        {
            write_bytes(position, BitConverter.GetBytes(value));
        }

        public void write_bytes(long position, byte[] value)
        {
            lock (lockObject)
            {
                file.Position = position;
                file.Write(value);
            }
        }

        //

        private readonly object lockObject = new object();

        private readonly WinFileIO file;
    }
}