using System;

namespace Renfield.SafeRedir.Services
{
  public class DateService
  {
    public string GetRange(DateTime? fromDate, DateTime? toDate)
    {
      if (fromDate == null && toDate == null)
        return "all records";

      var d1 = fromDate.GetValueOrDefault().ToString(Constants.DATE_FORMAT);
      var d2 = toDate.GetValueOrDefault().ToString(Constants.DATE_FORMAT);

      if (fromDate == null)
        return string.Format("records before {0}", d2);
      if (toDate == null)
        return string.Format("records after {0}", d1);
      return string.Format("records between {0} and {1}", d1, d2);
    }
  }
}