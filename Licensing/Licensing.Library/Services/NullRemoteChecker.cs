using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  /// <summary>
  ///   Implements the Null Object pattern for the RemoteChecker interface.
  /// </summary>
  public class NullRemoteChecker : RemoteChecker
  {
    public void Check(LicenseRegistration registration)
    {
      // do nothing
    }

    public void Submit(LicenseRegistration registration)
    {
      // do nothing
    }
  }
}