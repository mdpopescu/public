using Conway.Library.Models;

namespace Conway.Library.Contracts
{
    public interface Screen
    {
        void Clear();

        void Show(Coordinates coordinates, Color color);
    }
}