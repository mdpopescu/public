using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface Storage
  {
    LicenseRegistration Load(string password);
    void Save(string password, LicenseRegistration registration);
  }
}