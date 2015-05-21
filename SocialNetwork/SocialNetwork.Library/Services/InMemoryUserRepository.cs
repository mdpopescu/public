using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;

namespace SocialNetwork.Library.Services
{
  public class InMemoryUserRepository : UserRepository
  {
    public void AddFollower(string user, string other)
    {
      //
    }

    public IEnumerable<string> GetFollowers(string user)
    {
      return Enumerable.Empty<string>();
    }
  }
}