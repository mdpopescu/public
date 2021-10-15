using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Messaging.Library.Contracts;
using Messaging.Library.Helpers;
using Messaging.Library.Models;

namespace Messaging.Library.Services
{
    public class MessageBusFacade<TTransport> : IMessageBusFacade
    {
        public IObservable<MessageBase> Messages { get; }
        public IObservable<ErrorMessage> Errors { get; }

        public MessageBusFacade(IMessageSerializer<TTransport> serializer, IPubSub<TTransport> comms)
        {
            this.serializer = serializer;
            this.comms = comms;

            errorsSubject = new Subject<ErrorMessage>();

            Messages = comms
                .Messages
                .Select(SafeDeserialize)
                .Where(it => it != null)
                .Publish()
                .RefCount()!;
            Errors = Messages
                .OfType<ErrorMessage>()
                .Merge(errorsSubject.AsObservable());
        }

        public void Publish(MessageBase message)
        {
            var serialized = serializer.Serialize(message);
            comms.Publish(serialized);
        }

        //

        private readonly IMessageSerializer<TTransport> serializer;
        private readonly IPubSub<TTransport> comms;

        private readonly ISubject<ErrorMessage> errorsSubject;

        private MessageBase? SafeDeserialize(TTransport serialized)
        {
            var result = Safe.Call(() => serializer.Deserialize(serialized));
            if (result == null)
                AddError(serialized?.ToString());

            return result;
        }

        private void AddError(string? additionalInfo)
        {
            var errorMessage = new ErrorMessage(Guid.NewGuid(), null, null, 0, "Deserialization error", additionalInfo);
            errorsSubject.OnNext(errorMessage);
        }
    }
}