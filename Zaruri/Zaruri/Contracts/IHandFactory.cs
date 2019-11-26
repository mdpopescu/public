namespace Zaruri.Contracts
{
    public interface IHandFactory
    {
        Hand Create(int[] roll);
    }
}