namespace SocialNetwork.Library.Contracts
{
  public interface UserRepository
  {
    void AddFollower(string user, string other);
  }
}