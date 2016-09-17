using System;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.TwitterLibrary.Implementations
{
    public class TwitterChannel : Channel
    {
        public TwitterChannel(Tokens tokens)
        {
            this.tokens = tokens;
        }

        public void Send(Message message)
        {
            // TODO: this is not working correctly
            tokens.Statuses.Update(new StatusResponse { InReplyToScreenName = message.AccountId, Text = message.Text });
        }

        public IObservable<Message> Receive()
        {
            return tokens
                .Streaming
                .UserAsObservable()
                .OfType<StatusMessage>()
                .Select(ConvertMessage);
        }

        //

        private readonly Tokens tokens;

        private static Message ConvertMessage(StatusMessage message)
        {
            return new Message(message.Status.User.ScreenName, message.Status.Text);
        }
    }
}