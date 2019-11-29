using System;
using Zaruri.Models;
using Zaruri.Services;

namespace Zaruri.Contracts
{
    public interface IPlayerLogic
    {
        bool IsBroke(int amount);
        OutputWrapper<int> MakeBet(int amount);
        OutputWrapper<int[]> InitialRoll(int[] roll);
        OutputWrapper<int[]> FinalRoll(int[] roll, Indices indices, Func<int> rollDie);
        OutputWrapper<int> ComputeHand(int[] roll, int amount);
    }
}