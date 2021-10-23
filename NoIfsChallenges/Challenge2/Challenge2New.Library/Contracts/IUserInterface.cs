using Challenge2New.Library.Models;

namespace Challenge2New.Library.Contracts
{
    public interface IUserInterface
    {
        void SetEnabled(OperableControl control, bool value);
        void SetDisplay(string value);
    }
}