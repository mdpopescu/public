namespace Challenge2.Library.Contracts
{
    public interface UserInterface
    {
        void EnableStartStop();
        void DisableStartStop();

        void EnableReset();
        void DisableReset();

        void EnableHold();
        void DisableHold();

        void Display(string text);
    }
}