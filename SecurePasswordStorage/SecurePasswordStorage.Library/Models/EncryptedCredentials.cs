namespace SecurePasswordStorage.Library.Models
{
    public class EncryptedCredentials
    {
        public byte[] Encrypted { get; set; }
        public SecureHash Hashed { get; set; }
    }
}