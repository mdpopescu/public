using System.Collections.Generic;

namespace SocialNetwork.Library.Contracts
{
  public interface UserRepository
  {
    void AddFollower(string user, string other);
    IEnumerable<string> GetFollowers(string user);
  }
}