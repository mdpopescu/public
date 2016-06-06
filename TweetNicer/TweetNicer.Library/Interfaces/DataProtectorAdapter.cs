namespace TweetNicer.Library.Interfaces
{
    public interface DataProtectorAdapter
    {
        string EncryptForUser(string data);
        string EncryptForMachine(string data, string password);

        string DecryptForUser(string data);
        string DecryptForMachine(string data, string password);
    }
}