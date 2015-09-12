using System;
using System.Globalization;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public static Licenser Create(LicenseOptions options)
    {
      StringIO io = null;
      Encryptor encryptor = new RijndaelEncryptor(options.Password);
      Serializer<LicenseRegistration> serializer = new LicenseSerializer();

      return new Licenser(
        options,
        new SecureStorage(io, encryptor, serializer),
        new WinSys(),
        new WebRemote());
    }

    public Licenser(LicenseOptions options, Storage storage, Sys sys, Remote remote)
    {
      this.options = options;
      this.storage = storage;
      this.sys = sys;
      this.remote = remote;
    }

    public bool IsLicensed()
    {
      var registration = storage.Load();
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

      var registration = storage.Load();
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

    public bool ShouldRun()
    {
      return IsLicensed() || IsTrial();
    }

    //

    private readonly LicenseOptions options;
    private readonly Storage storage;
    private readonly Sys sys;
    private readonly Remote remote;

    private bool CheckRemoteResponse(LicenseRegistration registration)
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
      var address = string.Format("https://{0}?Key={1}&ProcessorId={2}", options.CheckUrl, key, processorId);

      return remote.Get(address);
    }

    private void UpdateExpirationDate(LicenseRegistration registration, DateTime expiration)
    {
      registration.Expiration = expiration;
      storage.Save(registration);
    }

    private void UpdateRemainingRuns(LicenseRegistration registration)
    {
      registration.Limits.Runs--;
      storage.Save(registration);
    }
  }
}