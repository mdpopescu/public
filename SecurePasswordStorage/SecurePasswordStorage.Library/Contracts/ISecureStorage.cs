using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ISecureStorage
    {
        void Save(Credentials credentials, byte[] secret);
        byte[] Load(Credentials credentials);
    }
}