using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface Storage
  {
    LicenserRegistration Load(string password);
    void Save(string password, LicenserRegistration registration);
  }
}