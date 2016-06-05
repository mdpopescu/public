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
                .FilterAsObservable(track => "hearthstone")
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

            var prefixLength = prefix.Length;

            var sb = new StringBuilder();

            do
            {
                var limit = Math.Min(text.Length, length - prefixLength);

                var current = text.Substring(0, limit);
                sb.Append(prefix + current);

                prefix = new string(' ', prefixLength);
                text = text.Substring(limit);
            } while (text != "");

            return sb.ToString();
        }
    }
}