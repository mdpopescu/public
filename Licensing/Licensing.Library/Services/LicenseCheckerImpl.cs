﻿using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class LicenseCheckerImpl : LicenseChecker
  {
    public bool IsLicensed { get; private set; }
    public bool IsTrial { get; private set; }

    public LicenseCheckerImpl(RemoteChecker remote, Validator validator)
    {
      this.remote = remote;
      this.validator = validator;
    }

    public void Check(LicenseRegistration registration)
    {
      SetIsLicensed(registration);
      SetIsTrial(registration);
    }

    //

    private readonly RemoteChecker remote;
    private readonly Validator validator;

    private void SetIsLicensed(LicenseRegistration registration)
    {
      IsLicensed = false;

      if (!validator.Isvalid(registration))
        return;

      var expiration = remote.Check(registration);
      if (!expiration.HasValue)
        return;

      registration.Expiration = expiration.Value;
      IsLicensed = DateTime.Today <= registration.Expiration;
    }

    private void SetIsTrial(LicenseRegistration registration)
    {
      if (IsLicensed)
        IsTrial = true;
      else if (registration == null)
        IsTrial = false;
      else if (registration.Limits == null)
        IsTrial = false;
      else if (registration.Limits.GetRemainingDays(registration.CreatedOn) <= 0)
        IsTrial = false;
      else if (registration.Limits.Runs == 0)
        IsTrial = false;
      else
        IsTrial = true;
    }
  }
}