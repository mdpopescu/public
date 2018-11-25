using Conway.Library.Models;

namespace Conway.Library.Contracts
{
    public interface Screen
    {
        void Clear();
        void ShowBlock(Coordinates coordinates);
    }
}