using System;
using System.Linq;
using System.Web.Mvc;
using Renfield.SafeRedir.Data;
using Renfield.SafeRedir.Models;

namespace Renfield.SafeRedir.Services
{
  public class BusinessLogic : Logic
  {
    public BusinessLogic(Repository repository, UniqueIdGenerator uniqueIdGenerator)
    {
      this.repository = repository;
      this.uniqueIdGenerator = uniqueIdGenerator;
    }

    public string CreateRedirect(string url, string safeUrl, int ttl)
    {
      var urlInfo = new UrlInfo
      {
        OriginalUrl = url,
        SafeUrl = safeUrl,
        ExpiresAt = SystemInfo.SystemClock().AddSeconds(ttl),
      };

      do
      {
        urlInfo.Id = uniqueIdGenerator.Generate();
        if (repository.GetUrlInfo(urlInfo.Id) != null)
          continue;

        try
        {
          repository.AddUrlInfo(urlInfo);
          repository.SaveChanges();
          return urlInfo.Id;
        }
        catch (Exception)
        {
          // get another unique id and try again
        }
      } while (true);
    }

    public RedirectResult GetUrl(string shortUrl)
    {
      var urlInfo = repository.GetUrlInfo(shortUrl);
      if (urlInfo == null)
        return null;

      var now = SystemInfo.SystemClock();
      var url = urlInfo.GetUrl(now);
      var permanent = url == urlInfo.SafeUrl;

      return new RedirectResult(url, permanent);
    }

    public SummaryInfo GetSummary()
    {
      var today = SystemInfo.SystemClock().Date;

      // do NOT call .ToList() here - it MUST be evaluated multiple times
      var records = repository
        .GetAll()
        .Select(it => it.ExpiresAt);

      return new SummaryInfo
      {
        Today = records.Count(it => it.Date == today),
        CurrentMonth = records.Count(it => it.Date.Month == today.Month && it.Date.Year == today.Year),
        CurrentYear = records.Count(it => it.Date.Year == today.Year),
        Overall = records.Count(),
      };
    }

    //

    private readonly Repository repository;
    private readonly UniqueIdGenerator uniqueIdGenerator;
  }
}