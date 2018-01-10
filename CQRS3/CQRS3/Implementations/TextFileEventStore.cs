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
            var serialized = Serialize(ev);
            File.AppendAllText(filename, serialized + Environment.NewLine);
        }

        //

        private readonly string filename;

        private IEnumerable<EventBase> ReadEvents() => File.ReadAllLines(filename).Select(Deserialize).ToList();

        private static string Serialize(EventBase ev)
        {
            switch (ev)
            {
                case Incremented _:
                    return "i";

                case Decremented _:
                    return "d";

                case Unchanged u:
                    return $"n {u.Reason}";

                default:
                    throw new Exception($"Internal error: unknown type {ev.GetType()}.");
            }
        }

        private static EventBase Deserialize(string line)
        {
            switch (line[0])
            {
                case 'i':
                    return new Incremented();

                case 'd':
                    return new Decremented();

                case 'n':
                    return new Unchanged { Reason = line.Length > 2 ? line.Substring(2) : "Cannot decrement below 0." };

                default:
                    throw new Exception($"Internal error: unknown serialized value {line}.");
            }
        }
    }
}