namespace SecurePasswordStorage.Library.Models
{
    public class User : Entity<UserKey>
    {
        public byte[] Salt { get; }
        public byte[] PasswordHash { get; }

        public User(UserKey key, byte[] salt, byte[] passwordHash)
            : base(key)
        {
            Salt = salt;
            PasswordHash = passwordHash;
        }
    }
}