using System;

namespace TimerApp.Contracts
{
    public interface UserInterface
    {
        void SetButtonText(string text);

        void ShowTime(TimeSpan ts);
    }
}