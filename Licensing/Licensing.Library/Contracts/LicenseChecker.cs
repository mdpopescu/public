using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface LicenseChecker
  {
    bool IsLicensed { get; }
    bool IsTrial { get; }

    void Check(LicenseRegistration registration);
  }
}