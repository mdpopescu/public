using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface IUserRepository : IRepository<User, UserKey>
    {
    }
}