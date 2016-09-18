namespace Elomen.TwitterLibrary.Models
{
    public class TwitterAccount
    {
        public long Id { get; private set; }
        public string Username { get; private set; }

        public TwitterAccount(long id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}