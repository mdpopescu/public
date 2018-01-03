using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CQRS3.Implementations.Events;
using CQRS3.Library.Contracts;
using CQRS3.Library.Helpers;

namespace CQRS3.Implementations
{
    public class TextFileEventStore : EventStore
    {
        public TextFileEventStore(string filename)
        {
            this.filename = filename;
        }

        public override IEnumerable<EventBase> Get() => Result.Execute(ReadEvents).OrElse(Enumerable.Empty<EventBase>());

        //

        protected override void Save(EventBase ev)
        {
            var serialized = ev is Incremented ? "i" : "d";
            File.AppendAllText(filename, serialized + Environment.NewLine);
        }

        //

        private readonly string filename;

        private IEnumerable<EventBase> ReadEvents() => File.ReadAllLines(filename).Select(Deserialize).ToList();

        private static EventBase Deserialize(string line) => line == "i" ? (EventBase) new Incremented() : new Decremented();
    }
}