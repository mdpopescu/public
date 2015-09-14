using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface Validator
  {
    bool Isvalid(LicenseRegistration registration);
  }
}