using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Challenge2.Rx.Helpers
{
    public static class ObservableLog
    {
        public static Action<string> Log { get; } = Console.WriteLine;
        public static int Outstanding { get; private set; }

        public static IObservable<T> Create<T>(string name, IObservable<T> source) => Observable
            .Create<T>(
                observer =>
                {
                    AddEvent(name, EventType.CREATE);
                    Outstanding++;
                    Log($"{name} subscribed, Outstanding = {Outstanding}.");

                    var subscription = source.Subscribe(
                        value =>
                        {
                            AddEvent(name, EventType.REGULAR);
                            observer.OnNext(value);
                        },
                        ex =>
                        {
                            AddEvent(name, EventType.ERROR);
                            Log($"{name} error: {ex.Message}");
                        },
                        () =>
                        {
                            AddEvent(name, EventType.COMPLETE);
                            Log($"{name} completed.");
                        }
                    );
                    return () =>
                    {
                        subscription.Dispose();

                        AddEvent(name, EventType.RELEASE);
                        Outstanding--;
                        Log($"{name} unsubscribed, Outstanding = {Outstanding}.");
                    };
                }
            )
            .Share();

        public static string[] GetTimeline()
        {
            var ordered = EVENTS.OrderBy(it => it.Time).ToArray();
            var moments = ordered.Select(it => it.Time).ToArray();
            var useInterval = TimeSpan.FromMilliseconds(250);
            var startAt = moments.First();
            var endAt = moments.Last();
            var intervalCount = (int)Math.Ceiling((endAt - startAt).TotalSeconds / useInterval.TotalSeconds);
            var sources = EVENTS.Select(it => it.Source).Distinct().ToArray();
            var matrix = sources.ToDictionary(it => it, _ => CreateArray(intervalCount + 1, '-'));

            var col = 0;
            for (var time = startAt; time < endAt + useInterval; time += useInterval, col++)
            {
                var relevant = EVENTS.Where(it => it.Time >= time && it.Time < time + useInterval).ToArray();
                foreach (var ev in relevant)
                    matrix[ev.Source][col] = EVENT_TYPE_MAP[ev.EventType];
            }

            var nameLen = sources.Max(it => it.Length);
            return sources.Select(name => $"{name.PadRight(nameLen)} {new string(matrix[name])}").ToArray();
        }

        //

        private static readonly List<Event> EVENTS = new List<Event>();

        private static void AddEvent(string source, EventType eventType) =>
            EVENTS.Add(new Event(source, eventType));

        private static T[] CreateArray<T>(int count, T value)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
                result[i] = value;

            return result;
        }

        //

        private enum EventType
        {
            CREATE,
            RELEASE,
            REGULAR,
            COMPLETE,
            ERROR,
        }

        private static readonly Dictionary<EventType, char> EVENT_TYPE_MAP = new Dictionary<EventType, char>
        {
            { EventType.CREATE, '^' },
            { EventType.RELEASE, 'v' },
            { EventType.REGULAR, 'X' },
            { EventType.COMPLETE, '|' },
            { EventType.ERROR, '!' },
        };

        private class Event
        {
            public DateTime Time { get; }
            public string Source { get; }
            public EventType EventType { get; }

            public Event(string source, EventType eventType)
            {
                Time = DateTime.Now;
                Source = source;
                EventType = eventType;
            }
        }
    }
}