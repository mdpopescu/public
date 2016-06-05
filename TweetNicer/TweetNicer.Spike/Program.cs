using System;
using System.Configuration;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
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

            //var session = OAuth.Authorize(settings["ConsumerKey"], settings["ConsumerSecret"]);
            //Process.Start(session.AuthorizeUri.AbsoluteUri);
            //Console.Write("Enter PIN: ");
            //var pin = Console.ReadLine();
            //var tokens = session.GetTokens(pin);

            using (tokens
                .Streaming
                .FilterAsObservable(track => "football")
                .OfType<StatusMessage>()
                .Subscribe(msg => Console.WriteLine(Align(msg.Status.User.ScreenName + ": ", msg.Status.Text))))
            {
                Console.ReadLine();
            }
        }

        private static string Align(string prefix, string text, int length = 80)
        {
            Debug.Assert(prefix != null);
            Debug.Assert(text != null);
            Debug.Assert(length > prefix.Length);

            var all = prefix + text;

            return all.Length <= length
                ? all
                : all.Substring(0, length) +
                  Align(new string(' ', prefix.Length), all.Substring(length), length);
        }
    }
}