using System.Web.Mvc;
using Renfield.SafeRedir.Models;

namespace Renfield.SafeRedir.Services
{
  public interface Logic
  {
    string CreateRedirect(string url, string safeUrl, int ttl);
    RedirectResult GetUrl(string shortUrl);
    SummaryInfo GetSummary();
  }
}