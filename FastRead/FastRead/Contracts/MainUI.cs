namespace FastRead.Contracts
{
    public interface MainUI
    {
        void EnterFullScreen();
        void LeaveFullScreen();

        void Display(string word);
    }
}