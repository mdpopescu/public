using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  /// <summary>
  /// Implements the Null Object pattern for the RemoteChecker interface.
  /// In particular, this class always returns an expiration date far into the future,
  /// thus faking a web service that considers all keys to be valid.
  /// </summary>
  public class NullRemoteChecker : RemoteChecker
  {
    public DateTime? Check(LicenseRegistration registration)
    {
      return new DateTime(9999, 12, 31);
    }

    public DateTime? Submit(LicenseRegistration registration)
    {
      return new DateTime(9999, 12, 31);
    }
  }
}