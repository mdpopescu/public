namespace Challenge2.Library.Contracts
{
    public interface UserInterface
    {
        bool StartStopEnabled { set; }
        bool ResetEnabled { set; }
        bool HoldEnabled { set; }

        string Text { set; }
    }
}