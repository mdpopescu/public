using System;
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

      var processorId = sys.GetProcessorId();
      try
      {
        var response = remote.Get(string.Format("{0}?Key={1}&ProcessorId={2}", options.CheckUrl, registration.Key, processorId));
      }
      catch
      {
        return false;
      }

      return false;
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
  }
}