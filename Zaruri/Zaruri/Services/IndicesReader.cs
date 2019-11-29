using System;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;

namespace Zaruri.Services
{
    public class IndicesReader : IIndicesReader
    {
        public IndicesReader(IWriter writer, IReader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public Indices ReadIndices()
        {
            while (true)
                try
                {
                    return InternalReadIndices();
                }
                catch
                {
                    // try again
                }
        }

        //

        private readonly IWriter writer;
        private readonly IReader reader;

        private Indices InternalReadIndices()
        {
            writer.Write("Enter the dice to keep (1 .. 5), separated with spaces: ");
            var line = reader.ReadLine();
            var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Select(Index.Create).ToArray();
            return Indices.Create(values);
        }
    }
}