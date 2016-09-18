namespace Elomen.Library.Model
{
    public class Account
    {
        public static readonly Account GUEST = new Account(0, "");

        public long Id { get; private set; }
        public string Username { get; private set; }

        public Account(long id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}