using System.Linq;
using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class HandFactory : IHandFactory
    {
        public Hand Create(int[] roll) => HANDS.Where(it => it.IsMatch(roll)).First();

        //

        private static readonly Hand[] HANDS =
        {
            new HighFlush(),
            new LowFlush(),
            new FiveOfAKind(),
            new FourOfAKind(),
            new FullHouse(),
            new ThreeOfAKind(),
            new TwoPairs(),
            new OnePair(),
            new Nothing(),
        };
    }
}