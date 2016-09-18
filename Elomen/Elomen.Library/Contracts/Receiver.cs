using System;

namespace Elomen.Library.Contracts
{
    public interface Receiver<out T>
    {
        IObservable<T> Receive();
    }
}