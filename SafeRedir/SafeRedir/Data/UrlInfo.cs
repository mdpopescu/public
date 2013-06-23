using System;

namespace Renfield.SafeRedir.Data
{
  public class UrlInfo
  {
    public string Id { get; set; }
    public string OriginalUrl { get; set; }
    public string SafeUrl { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string GetUrl(DateTime when)
    {
      return when >= ExpiresAt ? SafeUrl : OriginalUrl;
    }
  }
}