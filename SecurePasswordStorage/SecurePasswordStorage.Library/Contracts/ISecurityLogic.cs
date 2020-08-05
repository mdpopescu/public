using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ISecurityLogic
    {
        EncryptedCredentials GetEncryptedCredentials(Credentials loginCredentials, Credentials foreignCredentials);
    }
}