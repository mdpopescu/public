using System;

namespace CQRS3.Library.Helpers
{
    public class Void
    {
        public static Void Singleton { get; } = new Void();

        public static Func<Void> From(Action action)
        {
            return () =>
            {
                action();
                return Singleton;
            };
        }

        //

        private Void()
        {
            // do not allow any other instances
        }
    }
}