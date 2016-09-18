namespace Elomen.Library.Contracts
{
    public interface Sender<in T>
    {
        void Send(T message);
    }
}