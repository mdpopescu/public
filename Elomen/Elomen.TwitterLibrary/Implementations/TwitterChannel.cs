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
            // TODO: 1. This needs the "in reply to" id / name
            // TODO: 2. Sending the whole thing leads to "msg7" ->
            // TODO:    "I do not know what [@ElomenBot msg7] means" ->
            // TODO:    "I do not know what [I do not know what [@ElomenBot msg7] means.] means." -> ...
            tokens.Statuses.Update(status => message.Text);
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