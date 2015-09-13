using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface ResponseParser
  {
    RemoteResponse Parse(string response);
  }
}