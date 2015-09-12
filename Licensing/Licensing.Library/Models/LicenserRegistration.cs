using System;

namespace Renfield.Licensing.Library.Models
{
  [Serializable]
  public class LicenserRegistration
  {
    public DateTime CreatedOn { get; set; }
    public Limits Limits { get; set; }

    public string Key { get; set; }
    public string Name { get; set; }
    public string Contact { get; set; }
    public DateTime Expiration { get; set; }

    public bool IsValidLicense()
    {
      var isGuid = IsValidGuid(Key);
      var hasName = !string.IsNullOrWhiteSpace(Name);
      var hasContact = !string.IsNullOrWhiteSpace(Contact);

      return isGuid && hasName && hasContact && DateTime.Today <= Expiration;
    }

    //

    private static bool IsValidGuid(string s)
    {
      Guid guid;
      return Guid.TryParse(s + "", out guid);
    }
  }
}