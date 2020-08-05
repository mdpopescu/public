using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ISecureStorage
    {
        void Save(Credentials loginCredentials, Credentials foreignCredentials);
    }
}