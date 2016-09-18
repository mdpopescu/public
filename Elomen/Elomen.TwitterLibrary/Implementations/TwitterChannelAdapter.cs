using System;
using Elomen.Library.Contracts;
using Elomen.Library.Model;
using Elomen.TwitterLibrary.Models;

namespace Elomen.TwitterLibrary.Implementations
{
    public class TwitterChannelAdapter : Channel<Message>
    {
        public TwitterChannelAdapter(Channel<TwitterMessage> channel, Repository<long, TwitterAccount> repository)
        {
            this.channel = channel;
            this.repository = repository;
        }

        public void Send(Message message)
        {
            var account = repository.Find(message.Account.Id);
            channel.Send(new TwitterMessage(account, message.Text));
        }

        public IObservable<Message> Receive()
        {
            throw new NotImplementedException();
        }

        //

        private readonly Channel<TwitterMessage> channel;
        private readonly Repository<long, TwitterAccount> repository;
    }
}