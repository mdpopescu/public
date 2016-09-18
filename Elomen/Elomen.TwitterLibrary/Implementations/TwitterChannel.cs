using System;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.TwitterLibrary.Implementations
{
    // TODO: this needs a lot of refactoring, there is too much embedded business logic
    public class TwitterChannel : Channel
    {
        public TwitterChannel(Tokens tokens)
        {
            this.tokens = tokens;
        }

        public void Send(Message message)
        {
            tokens.Statuses.Update($"@{message.Account.Username} {message.Text}", message.Account.Id);
        }

        public IObservable<Message> Receive()
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

        private static Message ConvertMessage(StatusMessage message)
        {
            var account = new Account(message.Status.User.Id.GetValueOrDefault(), message.Status.User.ScreenName);
            return new Message(account, message.Status.Text.Substring(BOT_NAME.Length + 1).Replace("@", ""));
        }
    }
}