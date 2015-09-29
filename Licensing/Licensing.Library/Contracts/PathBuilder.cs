using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface PathBuilder
  {
    string GetPath(Details details);
  }
}