namespace Zaruri.Contracts
{
    public interface IPlayer
    {
        bool HasMoney();
        void MakeBet();
        void InitialRoll();
        void FinalRoll();
        void ComputeHand();
    }
}