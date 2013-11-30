using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models
{
  public class MonthModel
  {
    public int Year { get; private set; }
    public int Month { get; private set; }

    public string MonthName
    {
      get
      {
        return months[Month];
      }
    }

    public MonthModel(int year, int month)
    {
      Year = year;
      Month = month;
    }

    //

    private const string MONTH_NAMES = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec";
    private readonly string[] months = MONTH_NAMES.Split(',');
  }
}