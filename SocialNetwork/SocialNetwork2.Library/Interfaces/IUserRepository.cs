using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Library.Interfaces
{
    public interface IUserRepository
    {
        User CreateOrFind(string userName);
    }
}