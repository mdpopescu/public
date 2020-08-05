using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface IRepository
    {
        void SaveEncryptedCredentials(string username, EncryptedCredentials credentials);

        IUser LoadUser(string username, SecureHash passwordHash);
    }
}