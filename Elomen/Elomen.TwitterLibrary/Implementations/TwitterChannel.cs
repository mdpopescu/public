using System;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using Elomen.Library.Contracts;
using Elomen.TwitterLibrary.Models;

namespace Elomen.TwitterLibrary.Implementations
{
    public class TwitterChannel : Channel<TwitterMessage>
    {
        public TwitterChannel(Tokens tokens)
        {
            this.tokens = tokens;
        }

        public void Send(TwitterMessage message)
        {
            tokens.Statuses.Update($"@{message.Account.Username} {message.Text}", message.Account.Id);
        }

        public IObservable<TwitterMessage> Receive()
        {
            return tokens
                .Streaming
                .UserAsObservable()
                .OfType<StatusMessage>()
                .Where(m => m.Status.Text.StartsWith(BOT_NAME))
                .Select(ConvertMessage);
        }

        //

        private const string BOT_NAME = "@ElomenBot";

        private readonly Tokens tokens;

        private static TwitterMessage ConvertMessage(StatusMessage message)
        {
            var account = new TwitterAccount(message.Status.User);
            var text = message.Status.Text.Substring(BOT_NAME.Length + 1).Replace("@", "");

            return new TwitterMessage(account, text);
        }
    }
}