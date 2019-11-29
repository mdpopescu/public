using System;
using Zaruri.Models;

namespace Zaruri.Contracts
{
    public interface IPlayerLogic
    {
        bool IsBroke(int amount);
        (string, int) MakeBet(int amount);
        (string, int[]) InitialRoll(Func<int[]> rollDice);
        (string, int[]) FinalRoll(int[] roll, Func<Indices> getIndices, Func<int> rollDie);
        (string, int) ComputeHand(Hand hand, int amount);
    }
}