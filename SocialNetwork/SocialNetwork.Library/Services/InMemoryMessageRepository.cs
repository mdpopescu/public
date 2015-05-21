using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;
using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Services
{
  public class InMemoryMessageRepository : MessageRepository
  {
    public InMemoryMessageRepository()
    {
      messages = new List<Message>();
    }

    public void Add(Message message)
    {
      messages.Add(message);
    }

    public IEnumerable<Message> Get()
    {
      return messages.AsEnumerable();
    }

    //

    private readonly List<Message> messages;
  }
}