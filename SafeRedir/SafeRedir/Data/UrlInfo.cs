using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renfield.SafeRedir.Data
{
  public class UrlInfo
  {
    [Key]
    public string Id { get; set; }

    [Column(TypeName = "ntext")]
    public string OriginalUrl { get; set; }

    [Column(TypeName = "ntext")]
    public string SafeUrl { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string GetUrl(DateTime when)
    {
      return when >= ExpiresAt ? SafeUrl : OriginalUrl;
    }
  }
}