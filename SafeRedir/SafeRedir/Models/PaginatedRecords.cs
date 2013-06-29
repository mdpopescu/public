using System.Collections.Generic;
using Renfield.SafeRedir.Data;

namespace Renfield.SafeRedir.Models
{
  public class PaginatedRecords
  {
    public string DateRange { get; set; }
    public int PageCount { get; set; }
    public int CurrentPage { get; set; }
    public IEnumerable<UrlInfo> UrlInformation { get; set; }
  }
}