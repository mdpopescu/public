using System.Collections.Generic;
using Renfield.SafeRedir.Data;

namespace Renfield.SafeRedir.Models
{
  public class DisplayListModel
  {
    public string DateRange { get; set; }
    public IEnumerable<UrlInfo> UrlInformation { get; set; }
  }
}