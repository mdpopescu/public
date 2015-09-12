using System;

namespace Renfield.Licensing.Library.Models
{
  [Serializable]
  public class LicenserRegistration
  {
    public DateTime CreatedOn { get; set; }
    public Limits Limits { get; set; }

    public string LicenseKey { get; set; }
    public string LicenseName { get; set; }
    public string LicenseContact { get; set; }
    public DateTime LicenseExpiration { get; set; }
  }
}