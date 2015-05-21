using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Contracts
{
  public interface Repository
  {
    void Add(Message message);
  }
}