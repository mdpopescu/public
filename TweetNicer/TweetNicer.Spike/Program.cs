using System;
using System.Configuration;
using System.Reactive.Linq;
using System.Threading;
using CoreTweet;
using CoreTweet.Streaming;

namespace TweetNicer.Spike
{
    class Program
    {
        private static void Main(string[] args)
        {
            var settings = ConfigurationManager.AppSettings;

            var tokens = new Tokens
            {
                ConsumerKey = settings["ConsumerKey"],
                ConsumerSecret = settings["ConsumerSecret"],
                AccessToken = settings["AccessToken"],
                AccessTokenSecret = settings["AccessTokenSecret"],
            };

            //if (string.IsNullOrEmpty(Settings.Default.TwitterPin))
            //{
            //    var session = OAuth.Authorize(settings["ConsumerKey"], settings["ConsumerSecret"]);
            //    Process.Start(session.AuthorizeUri.AbsoluteUri);
            //    Console.Write("Enter PIN: ");
            //    Settings.Default.TwitterPin = Console.ReadLine();
            //    Settings.Default.Save();
            //}

            //var tokens = session.GetTokens(Settings.Default.TwitterPin);

            using (tokens
                .Streaming
                .FilterAsObservable(track => "football")
                .OfType<StatusMessage>()
                .Subscribe(x => Console.WriteLine($"{x.Status.User.ScreenName} says about football: {x.Status.Text}")))
            {
                Thread.Sleep(10 * 1000);
            }
        }
    }
}