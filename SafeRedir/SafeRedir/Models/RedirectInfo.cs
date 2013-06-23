using System.ComponentModel;

namespace Renfield.SafeRedir.Models
{
  public class RedirectInfo
  {
    [DisplayName("URL to shorten")]
    public string URL { get; set; }

    [DisplayName("Safe URL (used after TTL expires)")]
    public string SafeURL { get; set; }

    [DisplayName("Time-to-live (sec)")]
    public int TTL { get; set; }

    public RedirectInfo()
    {
      URL = "";
      SafeURL = Constants.DEFAULT_SAFE_URL;
      TTL = Constants.DEFAULT_TTL;
    }
  }
}