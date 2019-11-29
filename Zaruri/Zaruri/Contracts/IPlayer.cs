namespace Zaruri.Contracts
{
    public interface IPlayer
    {
        bool IsBroke();
        void MakeBet();
        void InitialRoll();
        void FinalRoll();
        void ComputeHand();
    }
}