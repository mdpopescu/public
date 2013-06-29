using System;

namespace Renfield.SafeRedir.Services
{
  public class DateService
  {
    public string GetRange(DateTime? fromDate, DateTime? toDate)
    {
      if (fromDate == null && toDate == null)
        return "all records";
      if (fromDate == null)
        return string.Format("records before {0:yyyy-MM-dd}", toDate);
      if (toDate == null)
        return string.Format("records after {0:yyyy-MM-dd}", fromDate);
      return string.Format("records between {0:yyyy-MM-dd} and {1:yyyy-MM-dd}", fromDate, toDate);
    }
  }
}