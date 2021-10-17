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

        public static IObservable<T> Create<T>(string name, Func<IObservable<T>> constructor) => Observable
            .Create<T>(observer => CreateSubscription(name, observer, constructor.Invoke()));

        private static Action CreateSubscription<T>(string name, IObserver<T> observer, IObservable<T> source)
        {
            name = GetActualName(name);

            AddEvent(name, EventType.CREATE);
            Outstanding++;
            Log($"{name} subscribed, Outstanding = {Outstanding}.");

            var subscription = source.LogSubscribe(name, observer);

            return () =>
            {
                subscription.Dispose();

                AddEvent(name, EventType.RELEASE);
                Outstanding--;
                Log($"{name} unsubscribed, Outstanding = {Outstanding}.");
            };
        }

        public static string[] GetTimeline()
        {
            var ordered = EVENTS.OrderBy(it => it.Time).ToArray();
            var moments = ordered.Select(it => it.Time).ToArray();
            var useInterval = TimeSpan.FromMilliseconds(250);
            var startAt = moments.First();
            var endAt = moments.Last();
            var intervalCount = (int)Math.Ceiling((endAt - startAt).TotalSeconds / useInterval.TotalSeconds);
            var matrix = SOURCES.ToDictionary(it => it, _ => new int[intervalCount + 1]);

            var col = 0;
            for (var time = startAt; time < endAt + useInterval; time += useInterval, col++)
            {
                var relevant = EVENTS.Where(it => it.Time >= time && it.Time < time + useInterval).ToArray();
                foreach (var ev in relevant)
                    matrix[ev.Source][col] |= (int)ev.EventType;
            }

            var nameLen = SOURCES.Max(it => it.Length);
            return SOURCES.Select(name => $"{name.PadRight(nameLen)} {GetLine(matrix[name])}").ToArray();
        }

        //

        private const string MAP = "-^vOX&67|9ABCDEF!HIJKLMNOPQRSTU";

        private static readonly List<string> SOURCES = new List<string>();
        private static readonly List<Event> EVENTS = new List<Event>();

        private static string GetActualName(string name)
        {
            var index = SOURCES.Count;
            var suffix = 0;
            var actualName = name;

            while (SOURCES.Contains(actualName))
            {
                index = SOURCES.IndexOf(actualName) + 1;
                suffix++;
                actualName = $"{name}-{suffix}";
            }

            SOURCES.Insert(index, actualName);
            return actualName;
        }

        private static IDisposable LogSubscribe<T>(this IObservable<T> source, string name, IObserver<T> observer) => source
            .Subscribe(
                value =>
                {
                    AddEvent(name, EventType.REGULAR);
                    observer.OnNext(value);
                },
                ex =>
                {
                    Log($"{name} error: {ex.Message}");
                    AddEvent(name, EventType.ERROR);
                    observer.OnError(ex);
                },
                () =>
                {
                    Log($"{name} completed.");
                    AddEvent(name, EventType.COMPLETE);
                    observer.OnCompleted();
                }
            );

        private static void AddEvent(string source, EventType eventType) =>
            EVENTS.Add(new Event(source, eventType));

        private static string GetLine(IEnumerable<int> eventTypes) =>
            new string(eventTypes.Select(it => MAP[it]).ToArray());

        //

        [Flags]
        private enum EventType
        {
            CREATE = 1,
            RELEASE = 2,
            REGULAR = 4,
            COMPLETE = 8,
            ERROR = 16,
        }

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