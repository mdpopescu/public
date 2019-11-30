using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class Game
    {
        public Game(IPlayer player)
        {
            this.player = player;
        }

        public bool CanContinue() => player.HasMoney();

        public void Round()
        {
            player.MakeBet();
            player.InitialRoll();
            player.FinalRoll();
            player.ComputeHand();
        }

        //

        private readonly IPlayer player;
    }
}