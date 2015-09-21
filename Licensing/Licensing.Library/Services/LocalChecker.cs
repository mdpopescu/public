using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class LocalChecker : LicenseChecker
  {
    public LocalChecker(LicenseChecker remote, Validator validator)
    {
      this.remote = remote;
      this.validator = validator;
    }

    public LicenseStatus Check(LicenseRegistration registration)
    {
      var status = new LicenseStatus();
      status.IsLicensed = GetIsLicensed(registration);
      status.IsTrial = status.IsLicensed || GetIsTrial(registration);

      return status;
    }

    public void Submit(LicenseRegistration registration)
    {
      if (validator.Isvalid(registration))
        remote.Submit(registration);
    }

    //

    private readonly LicenseChecker remote;
    private readonly Validator validator;

    private bool GetIsLicensed(LicenseRegistration registration)
    {
      if (!validator.Isvalid(registration))
        return false;

      var status = remote.Check(registration);
      return status.IsLicensed;
    }

    private bool GetIsTrial(LicenseRegistration registration)
    {
      if (registration == null)
        return false;
      if (registration.Limits == null)
        return false;
      if (registration.Limits.GetRemainingDays(registration.CreatedOn) <= 0)
        return false;
      if (registration.Limits.Runs == 0)
        return false;
      if (registration.Expiration < DateTime.Today)
        return false;
      return true;
    }
  }
}