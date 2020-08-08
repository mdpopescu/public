using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ISecretRepository : IRepository<SecretData, UserKey>
    {
    }
}