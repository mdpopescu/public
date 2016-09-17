using System;
using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface Receiver
    {
        IObservable<Message> Receive();
    }
}