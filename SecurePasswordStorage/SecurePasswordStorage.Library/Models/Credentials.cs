namespace SecurePasswordStorage.Library.Models
{
    public class Credentials : Entity<UserKey>
    {
        public string Password { get; }

        public Credentials(UserKey key, string password)
            : base(key)
        {
            Password = password;
        }
    }
}