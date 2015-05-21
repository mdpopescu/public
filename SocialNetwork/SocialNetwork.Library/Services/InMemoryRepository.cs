using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;
using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Services
{
  public class InMemoryRepository : Repository
  {
    public void Add(Message message)
    {
      //
    }

    public IEnumerable<Message> Get()
    {
      return Enumerable.Empty<Message>();
    }
  }
}