namespace Elomen.Library.Model
{
    public class Account
    {
        public long Id { get; }
        public string Username { get; }

        public Account(long id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}