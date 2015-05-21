using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;
using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Services
{
  public class CommandHandler : API
  {
    public CommandHandler(Repository repository)
    {
      this.repository = repository;
    }

    public void Post(string user, string message)
    {
      repository.Add(new Message(user, message));
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

    //

    private readonly Repository repository;
  }
}