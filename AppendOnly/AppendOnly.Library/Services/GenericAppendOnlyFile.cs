using System.Collections.Generic;
using System.Linq;
using Renfield.AppendOnly.Library.Contracts;

namespace Renfield.AppendOnly.Library.Services
{
    public class GenericAppendOnlyFile<T> : GenericAppendOnly<T>
    {
        public long[] GetIndex() => file.GetIndex();

        public GenericAppendOnlyFile(LowLevelAppendOnlyFile file, SerializationEngine serializationEngine)
        {
            this.file = file;
            this.serializationEngine = serializationEngine;
        }

        public void Append(T record)
        {
            var bytes = serializationEngine.Serialize(record);
            file.Append(bytes);
        }

        public T Read(int i)
        {
            var bytes = file.Read(i);
            return serializationEngine.Deserialize<T>(bytes);
        }

        public IEnumerable<T> ReadFrom(int i)
        {
            var bytesList = file.ReadFrom(i);
            return bytesList.Select(bytes => serializationEngine.Deserialize<T>(bytes));
        }

        //

        private readonly LowLevelAppendOnlyFile file;
        private readonly SerializationEngine serializationEngine;
    }
}