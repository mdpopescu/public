using System;
using Inventory2.Library.Contracts;
using Inventory2.Library.Core;

namespace Inventory2.Library.Shell
{
    public class EventSerializer : BinarySerializer<EventBase>
    {
        public byte[] Serialize(EventBase value)
        {
            //
            return new byte[0];
        }

        public EventBase Deserialize(byte[] serialized)
        {
            return null;
        }
    }
}