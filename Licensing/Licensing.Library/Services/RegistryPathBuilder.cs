using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class RegistryPathBuilder : PathBuilder
  {
    public string GetPath(Details details)
    {
      return string.Format(@"Software\{0}\{1}", details.Company, details.Product);
    }
  }
}