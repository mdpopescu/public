using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;

namespace SocialNetwork.Library.Services
{
  public class CommandHandler : API
  {
    public void Post(string user, string message)
    {
      //
    }

    public IEnumerable<string> Read(string user)
    {
      return Enumerable.Empty<string>();
    }

    public void Follow(string user, string other)
    {
      //
    }

    public IEnumerable<string> Wall(string user)
    {
      return Enumerable.Empty<string>();
    }
  }
}