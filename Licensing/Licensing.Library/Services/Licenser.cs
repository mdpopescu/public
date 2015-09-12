using System;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class Licenser
  {
    public Licenser(LicenserOptions options, Storage storage)
    {
      this.options = options;
      this.storage = storage;
    }

    public bool IsValid()
    {
      var registration = storage.Load(options.Password);
      if (registration == null)
        return false;

      var isGuid = IsValidGuid(registration.Key);
      var hasName = !string.IsNullOrWhiteSpace(registration.Name);
      var hasContact = !string.IsNullOrWhiteSpace(registration.Contact);

      return isGuid && hasName && hasContact;
    }

    //

    private readonly LicenserOptions options;
    private readonly Storage storage;

    private static bool IsValidGuid(string s)
    {
      Guid guid;
      return Guid.TryParse(s + "", out guid);
    }
  }
}