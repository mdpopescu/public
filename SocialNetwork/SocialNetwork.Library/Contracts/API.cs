using System.Collections.Generic;

namespace SocialNetwork.Library.Contracts
{
  public interface API
  {
    void Post(string user, string message);
    IEnumerable<string> Read(string user);
    void Follow(string user, string other);
    IEnumerable<string> Wall(string user);
  }
}