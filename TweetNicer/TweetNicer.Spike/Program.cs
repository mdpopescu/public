using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
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

            var tokens = GetTokens(settings["ConsumerKey"], settings["ConsumerSecret"]);

            //var tokens = new Tokens
            //{
            //    ConsumerKey = settings["ConsumerKey"],
            //    ConsumerSecret = settings["ConsumerSecret"],
            //    AccessToken = settings["AccessToken"],
            //    AccessTokenSecret = settings["AccessTokenSecret"],
            //};

            //var session = OAuth.Authorize(settings["ConsumerKey"], settings["ConsumerSecret"]);
            //Process.Start(session.AuthorizeUri.AbsoluteUri);
            //Console.Write("Enter PIN: ");
            //var pin = Console.ReadLine();
            //var tokens = session.GetTokens(pin);

            var stream = tokens.Streaming.UserAsObservable().Publish();

            //stream.OfType<FriendsMessage>()
            //    .Subscribe(x => Console.WriteLine("Following: " + string.Join(", ", x)));

            stream.OfType<StatusMessage>()
                .Subscribe(x => Console.WriteLine(Align(x.Status.User.ScreenName + ": ", x.Status.Text)));

            using (stream.Connect())
                Console.ReadLine();

            //var stream = tokens
            //    .Streaming
            //    .FilterAsObservable(track => "football")
            //    .OfType<StatusMessage>();

            //using (stream.Subscribe(msg => Console.WriteLine(Align(msg.Status.User.ScreenName + ": ", msg.Status.Text))))
            //{
            //    Console.ReadLine();
            //}
        }

        private static Tokens GetTokens(string consumerKey, string consumerSecret)
        {
            try
            {
                var userSecret = File.ReadAllLines("user.secret").Select(Decrypt).ToArray();
                return new Tokens
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = userSecret[0],
                    AccessTokenSecret = userSecret[1],
                };
            }
            catch (Exception)
            {
                var session = OAuth.Authorize(consumerKey, consumerSecret);
                Process.Start(session.AuthorizeUri.AbsoluteUri);
                Console.Write("Enter PIN: ");
                var pin = Console.ReadLine();
                var tokens = session.GetTokens(pin);

                var userSecret = new[] { tokens.AccessToken, tokens.AccessTokenSecret };
                File.WriteAllLines("user.secret", userSecret.Select(Encrypt));

                return tokens;
            }
        }

        private static string Decrypt(string text)
        {
            var encrypted = Convert.FromBase64String(text);
            var plaintext = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plaintext);
        }

        private static string Encrypt(string text)
        {
            var plaintext = Encoding.UTF8.GetBytes(text);
            var encrypted = ProtectedData.Protect(plaintext, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
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