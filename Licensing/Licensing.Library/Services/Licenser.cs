using System;
using System.Globalization;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public Licenser(LicenserOptions options, Storage storage, Sys sys, Remote remote)
    {
      this.options = options;
      this.storage = storage;
      this.sys = sys;
      this.remote = remote;
    }

    public bool IsLicensed()
    {
      var registration = storage.Load(options.Password);
      if (registration == null)
        return false;

      if (!registration.IsValidLicense())
        return false;

      // only check remotely if there is a CheckUrl
      if (string.IsNullOrWhiteSpace(options.CheckUrl))
        return true;

      return CheckRemoteResponse(registration);
    }

    public bool IsTrial()
    {
      if (IsLicensed())
        return true;

      var registration = storage.Load(options.Password);
      if (registration == null)
        return false;

      if (registration.Limits == null)
        return false;

      if (registration.Limits.Days != -1 && registration.CreatedOn.AddDays(registration.Limits.Days) < DateTime.Today)
        return false;

      if (registration.Limits.Runs == 0)
        return false;

      if (registration.Limits.Runs > 0)
        UpdateRemainingRuns(registration);

      return true;
    }

    //

    private readonly LicenserOptions options;
    private readonly Storage storage;
    private readonly Sys sys;
    private readonly Remote remote;

    private bool CheckRemoteResponse(LicenserRegistration registration)
    {
      try
      {
        var response = GetRemoteResponse(registration.Key);
        var parts = response.Split(' ');
        if (parts[0] != registration.Key)
          return false;

        var expiration = DateTime.ParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        UpdateExpirationDate(registration, expiration);

        return DateTime.Today <= expiration;
      }
      catch
      {
        return false;
      }
    }

    private string GetRemoteResponse(string key)
    {
      var processorId = sys.GetProcessorId();
      var address = string.Format("{0}?Key={1}&ProcessorId={2}", options.CheckUrl, key, processorId);

      return remote.Get(address);
    }

    private void UpdateExpirationDate(LicenserRegistration registration, DateTime expiration)
    {
      registration.Expiration = expiration;
      storage.Save(options.Password, registration);
    }

    private void UpdateRemainingRuns(LicenserRegistration registration)
    {
      registration.Limits.Runs--;
      storage.Save(options.Password, registration);
    }
  }
}