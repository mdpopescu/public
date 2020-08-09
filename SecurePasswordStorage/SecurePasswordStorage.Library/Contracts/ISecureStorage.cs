using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ISecureStorage
    {
        void SaveUser(Credentials credentials);
        User LoadUser(Credentials credentials);
        bool CheckUser(Credentials credentials);

        void SaveSecret(Credentials credentials, byte[] secret);
        byte[] LoadSecret(Credentials credentials);
    }
}