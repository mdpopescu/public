using System;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Contracts
{
  public interface RemoteChecker
  {
    DateTime? Check(LicenseRegistration registration);
    void Submit(LicenseRegistration registration);
  }
}