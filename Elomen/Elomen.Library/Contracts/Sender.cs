using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface Sender
    {
        void Send(Message message);
    }
}