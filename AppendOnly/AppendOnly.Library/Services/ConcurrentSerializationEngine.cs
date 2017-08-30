using Renfield.AppendOnly.Library.Contracts;

namespace Renfield.AppendOnly.Library.Services
{
    public class ConcurrentSerializationEngine : SerializationEngine
    {
        public ConcurrentSerializationEngine(SerializationEngine engine)
        {
            this.engine = engine;
        }

        public byte[] Serialize<T>(T value)
        {
            lock (lockObject)
                return engine.Serialize(value);
        }

        public T Deserialize<T>(byte[] buffer)
        {
            lock (lockObject)
                return engine.Deserialize<T>(buffer);
        }

        //

        private readonly object lockObject = new object();

        private readonly SerializationEngine engine;
    }
}