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

      var isGuid = IsValidGuid(registration.Key);
      var hasName = !string.IsNullOrWhiteSpace(registration.Name);
      var hasContact = !string.IsNullOrWhiteSpace(registration.Contact);

      if (!(isGuid && hasName && hasContact && DateTime.Today <= registration.Expiration))
        return false;

      // only check remotely if there is a CheckUrl
      if (string.IsNullOrWhiteSpace(options.CheckUrl))
        return true;

      try
      {
        var response = GetRemoteResponse(registration.Key);
        var parts = response.Split(' ');
        if (parts[0] != registration.Key)
          return false;

        var expiration = DateTime.ParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        UpdateRegistration(registration, expiration);

        return expiration >= DateTime.Today;
      }
      catch
      {
        return false;
      }
    }

    //

    private readonly LicenserOptions options;
    private readonly Storage storage;
    private readonly Sys sys;
    private readonly Remote remote;

    private static bool IsValidGuid(string s)
    {
      Guid guid;
      return Guid.TryParse(s + "", out guid);
    }

    private string GetRemoteResponse(string key)
    {
      var processorId = sys.GetProcessorId();
      return remote.Get(string.Format("{0}?Key={1}&ProcessorId={2}", options.CheckUrl, key, processorId));
    }

    private void UpdateRegistration(LicenserRegistration registration, DateTime expiration)
    {
      registration.Expiration = expiration;
      storage.Save(options.Password, registration);
    }
  }
}