namespace SocialNetwork2.Library.Interfaces
{
    public interface IUserRepository
    {
        IUser CreateOrFind(string userName);
    }
}