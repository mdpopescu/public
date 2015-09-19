using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface LicenseChecker
  {
    bool IsLicensed { get; }
    bool IsTrial { get; }

    /// <summary>
    ///   Checks the registration and sets <c>IsLicensed</c> / <c>IsTrial</c> accordingly.
    /// </summary>
    /// <param name="registration">The registration details.</param>
    void Check(LicenseRegistration registration);

    /// <summary>
    ///   Submits the registration details to the remote server.
    /// </summary>
    /// <param name="registration">The registration details.</param>
    void Submit(LicenseRegistration registration);
  }
}