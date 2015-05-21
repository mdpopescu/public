using System.Collections.Generic;
using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Contracts
{
  public interface MessageRepository
  {
    void Add(Message message);
    IEnumerable<Message> Get();
  }
}