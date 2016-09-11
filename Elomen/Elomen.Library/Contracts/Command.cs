using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface Command
    {
        void Execute(Account account);
    }
}