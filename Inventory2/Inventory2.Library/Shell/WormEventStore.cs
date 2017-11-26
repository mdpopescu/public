using System.Collections.Generic;
using System.Linq;
using Inventory2.Library.Contracts;
using Inventory2.Library.Core;

namespace Inventory2.Library.Shell
{
    public class WormEventStore : EventStore
    {
        public WormEventStore(WORMStream stream, BinarySerializer<EventBase> serializer)
        {
            this.stream = stream;
            this.serializer = serializer;
        }

        public void Add(EventBase ev) =>
            stream.Append(serializer.Serialize(ev));

        public IEnumerable<EventBase> GetAll() =>
            stream.ReadAll().Select(serializer.Deserialize);

        //

        private readonly WORMStream stream;
        private readonly BinarySerializer<EventBase> serializer;
    }
}