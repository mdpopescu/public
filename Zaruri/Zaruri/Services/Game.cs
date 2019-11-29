using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class Game
    {
        public Game(IPlayer player)
        {
            this.player = player;
        }

        public bool IsOver() => player.IsBroke();

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