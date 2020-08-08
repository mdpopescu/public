namespace SecurePasswordStorage.Library.Models
{
    public class User : Entity<UserKey>
    {
        public byte[] PasswordHash { get; }

        public User(UserKey key, byte[] passwordHash)
            : base(key)
        {
            PasswordHash = passwordHash;
        }
    }
}