using System;
using Zaruri.Models;

namespace Zaruri.Contracts
{
    public interface IPlayerLogic
    {
        bool IsBroke(int amount);
        (string, int) MakeBet(int amount);
        (string, int[]) InitialRoll(int[] roll);
        (string, int[]) FinalRoll(int[] roll, Indices indices, Func<int> rollDie);
        (string, int) ComputeHand(Hand hand, int amount);
    }
}