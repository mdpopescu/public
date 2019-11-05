namespace TimerApp.Contracts
{
    public interface TimerState
    {
        TimerState StartStop();

        void DisplayTime();
    }
}