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
  }
}