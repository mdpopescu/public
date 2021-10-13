using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Messaging.Library.Contracts;
using Messaging.Library.Models;

namespace Messaging.Library.Services
{
    public class MessageBusFacade<TTransport> : IMessageBusFacade
    {
        public MessageBusFacade(IMessageSerializer<TTransport> serializer, IPubSub<TTransport> comms)
        {
            this.serializer = serializer;
            this.comms = comms;

            errorsSubject = new Subject<IErrorMessage>();

            IObservable<IMessage> InternalGetMessages() => comms.Messages.Select(SafeDeserialize).Where(it => it != null).Publish().RefCount()!;
            IObservable<IErrorMessage> InternalGetErrors() => GetMessages().OfType<IErrorMessage>().Merge(errorsSubject.AsObservable());

            messages = new Lazy<IObservable<IMessage>>(InternalGetMessages);
            errors = new Lazy<IObservable<IErrorMessage>>(InternalGetErrors);
        }

        public void Publish(IMessage message)
        {
            var serialized = serializer.Serialize(message);
            comms.Publish(serialized);
        }

        public IObservable<IMessage> GetMessages() => messages.Value;
        public IObservable<IErrorMessage> GetErrors() => errors.Value;

        //

        private readonly IMessageSerializer<TTransport> serializer;
        private readonly IPubSub<TTransport> comms;

        private readonly ISubject<IErrorMessage> errorsSubject;

        private readonly Lazy<IObservable<IMessage>> messages;
        private readonly Lazy<IObservable<IErrorMessage>> errors;

        private IMessage? SafeDeserialize(TTransport serialized)
        {
            try
            {
                var result = serializer.Deserialize(serialized);
                if (result == null)
                    AddError(serialized?.ToString());
                return result;
            }
            catch
            {
                AddError(serialized?.ToString());
                return null;
            }
        }

        private void AddError(string? additionalInfo)
        {
            var errorMessage = new ErrorMessage(Guid.NewGuid(), null, null, 0, "Deserialization error", additionalInfo);
            errorsSubject.OnNext(errorMessage);
        }
    }
}