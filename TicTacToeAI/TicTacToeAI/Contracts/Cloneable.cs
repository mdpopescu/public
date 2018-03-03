namespace TicTacToeAI.Contracts
{
    public interface Cloneable<out T>
    {
        T Clone();
    }
}