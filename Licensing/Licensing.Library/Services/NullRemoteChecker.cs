using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  /// <summary>
  ///   Implements the Null Object pattern for the RemoteChecker interface.
  /// </summary>
  public class NullRemoteChecker : LicenseChecker
  {
    public LicenseStatus Check(LicenseRegistration registration)
    {
      return new LicenseStatus {IsLicensed = true, IsTrial = true};
    }

    public void Submit(LicenseRegistration registration)
    {
      // do nothing
    }
  }
}