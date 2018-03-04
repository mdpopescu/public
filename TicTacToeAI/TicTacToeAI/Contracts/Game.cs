namespace TicTacToeAI.Contracts
{
    public interface Game
    {
        float? Score { get; }

        bool HasEnded();
        float[] GetState();
        bool TryMove(object move);
    }
}