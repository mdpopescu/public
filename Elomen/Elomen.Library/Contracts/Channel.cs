namespace Elomen.Library.Contracts
{
    public interface Channel<T> : Sender<T>, Receiver<T>
    {
    }
}