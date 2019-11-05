using System;

namespace TimerApp.Contracts
{
    public interface UserInterface
    {
        void ChangeButtonToStart();
        void ChangeButtonToStop();
        void ChangeButtonToReset();

        void ShowTime(TimeSpan ts);
    }
}