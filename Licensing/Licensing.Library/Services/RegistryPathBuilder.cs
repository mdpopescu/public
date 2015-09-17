using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class RegistryPathBuilder : PathBuilder
  {
    public string GetPath(string company, string product)
    {
      return string.Format(@"Software\{0}\{1}", company, product);
    }
  }
}