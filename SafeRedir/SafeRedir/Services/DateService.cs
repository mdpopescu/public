using System;

namespace Renfield.SafeRedir.Services
{
  public class DateService
  {
    public string GetRange(DateTime fromDate, DateTime toDate)
    {
      if (fromDate == DateTime.MinValue && toDate == DateTime.MaxValue)
        return "all records";
      if (fromDate == DateTime.MinValue)
        return string.Format("records before {0:yyyy-MM-dd}", toDate);
      if (toDate == DateTime.MaxValue)
        return string.Format("records after {0:yyyy-MM-dd}", fromDate);
      return string.Format("records between {0:yyyy-MM-dd} and {1:yyyy-MM-dd}", fromDate, toDate);
    }
  }
}