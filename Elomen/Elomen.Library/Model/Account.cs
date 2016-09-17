namespace Elomen.Library.Model
{
    public class Account
    {
        public static readonly Account GUEST = new Account("");

        public string Id { get; private set; }

        public Account(string id)
        {
            Id = id;
        }
    }
}