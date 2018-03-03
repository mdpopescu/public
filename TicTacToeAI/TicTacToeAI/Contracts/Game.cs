namespace TicTacToeAI.Contracts
{
    public interface Game
    {
        float? Score { get; }

        bool HasEnded();
        float[] GetState();
        void Update(float[] values);
    }
}