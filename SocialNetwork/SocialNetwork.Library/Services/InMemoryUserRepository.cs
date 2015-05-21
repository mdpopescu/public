using System.Collections.Generic;
using SocialNetwork.Library.Contracts;

namespace SocialNetwork.Library.Services
{
  public class InMemoryUserRepository : UserRepository
  {
    public InMemoryUserRepository()
    {
      followed = new Dictionary<string, List<string>>();
    }

    public void AddFollower(string user, string other)
    {
      var list = GetList(user);
      if (!list.Contains(other))
        list.Add(other);
      followed[user] = list;
    }

    public IEnumerable<string> GetFollowers(string user)
    {
      return GetList(user);
    }

    //

    private readonly Dictionary<string, List<string>> followed;

    private List<string> GetList(string key)
    {
      return followed.ContainsKey(key) ? followed[key] : new List<string>();
    }
  }
}