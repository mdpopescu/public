using System;
using System.Collections.Generic;

namespace Renfield.Licensing.Library.Models
{
  [Serializable]
  public class LicenseRegistration
  {
    public LicenseRegistration()
    {
      CreatedOn = DateTime.Today;
      Limits = new Limits();
    }

    public DateTime CreatedOn { get; set; }
    public Limits Limits { get; set; }

    public string Key { get; set; }
    public string Name { get; set; }
    public string Contact { get; set; }
    public string ProcessorId { get; set; }
    public DateTime Expiration { get; set; }

    public IEnumerable<KeyValuePair<string, string>> GetLicenseFields()
    {
      yield return new KeyValuePair<string, string>("Key", Key);
      yield return new KeyValuePair<string, string>("Name", Name);
      yield return new KeyValuePair<string, string>("Contact", Contact);
      yield return new KeyValuePair<string, string>("ProcessorId", ProcessorId);
      yield return new KeyValuePair<string, string>("Expiration", Expiration.ToString("yyyy-MM-dd"));
    }
  }
}