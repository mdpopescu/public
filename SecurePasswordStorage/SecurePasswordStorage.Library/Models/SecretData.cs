namespace SecurePasswordStorage.Library.Models
{
    public class SecretData : Entity<UserKey>
    {
        public byte[] EncryptedSecret { get; }
        public byte[] VerificationHash { get; }

        public SecretData(UserKey key, byte[] encryptedSecret, byte[] verificationHash)
            : base(key)
        {
            EncryptedSecret = encryptedSecret;
            VerificationHash = verificationHash;
        }
    }
}