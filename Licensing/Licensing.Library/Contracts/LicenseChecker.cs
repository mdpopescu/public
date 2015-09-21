using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface LicenseChecker
  {
    /// <summary>
    ///   Checks the registration.
    /// </summary>
    /// <param name="registration">The registration details.</param>
    /// <returns>The status (is licensed / is trial).</returns>
    LicenseStatus Check(LicenseRegistration registration);

    /// <summary>
    ///   Submits the registration details to the remote server.
    /// </summary>
    /// <param name="registration">The registration details.</param>
    void Submit(LicenseRegistration registration);
  }
}