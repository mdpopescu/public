using System;
using System.Web.Mvc;
using Renfield.SafeRedir.Models;

namespace Renfield.SafeRedir.Services
{
  public interface Logic
  {
    string CreateRedirect(string url, string safeUrl, int ttl);
    RedirectResult GetUrl(string shortUrl);
    SummaryInfo GetSummary();

    /// <summary>
    ///   Returns a specific 10-record page
    /// </summary>
    /// <param name="page"> Page number, 1-based </param>
    /// <param name="fromDate"> Start date for selection </param>
    /// <param name="toDate"> End date for selection </param>
    /// <returns> </returns>
    PaginatedRecords GetRecords(int? page, DateTime? fromDate, DateTime? toDate);
  }
}