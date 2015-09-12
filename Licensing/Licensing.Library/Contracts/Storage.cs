using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface Storage
  {
    LicenseRegistration Load();
    void Save(LicenseRegistration registration);
  }
}