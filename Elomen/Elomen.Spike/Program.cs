using System;
using System.Reactive.Linq;
using CoreTweet.Streaming;
using Elomen.Spike.Implementations;

namespace Elomen.Spike
{
    internal class Program
    {
        private const string FILENAME = "settings.dat";

        private static void Main()
        {
            var tokenLoader = new TokenLoader(FILENAME, new ConsoleApprover());

            var tokens = tokenLoader.Load();

            var stream = tokens
                .Streaming
                .UserAsObservable()
                .OfType<StatusMessage>();

            using (stream.Subscribe(msg => Console.WriteLine(msg.Status.User.ScreenName + ": " + msg.Status.Text)))
            {
                Console.ReadLine();
            }
        }
    }
}