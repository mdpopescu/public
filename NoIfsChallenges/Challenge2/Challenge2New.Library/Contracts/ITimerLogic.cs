using System;

namespace Challenge2New.Library.Contracts
{
    public interface ITimerLogic : IDisposable
    {
        void Initialize(IUserInterface ui);
    }
}