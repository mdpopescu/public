using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Renfield.SafeRedir.Data;
using Renfield.SafeRedir.Models;

namespace Renfield.SafeRedir.Services
{
  public class BusinessLogic : Logic
  {
    public BusinessLogic(Repository repository, UniqueIdGenerator uniqueIdGenerator, DateService dateService)
    {
      this.repository = repository;
      this.uniqueIdGenerator = uniqueIdGenerator;
      this.dateService = dateService;
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

    public PaginatedRecords GetRecords(int? page, DateTime? fromDate, DateTime? toDate)
    {
      var relevantRecords = GetRecordsInRange(fromDate, toDate);

      var pageCount = (relevantRecords.Count() - 1) / Constants.PAGE_SIZE + 1;
      var fixedPage = FixPage(page, pageCount);

      var records = relevantRecords
        .OrderByDescending(it => it.ExpiresAt)
        .Skip((fixedPage - 1) * Constants.PAGE_SIZE)
        .Take(Constants.PAGE_SIZE)
        .ToList();

      return new PaginatedRecords
      {
        FromDate = fromDate,
        ToDate = toDate,
        PageCount = pageCount,
        CurrentPage = fixedPage,
        DateRange = dateService.GetRange(fromDate, toDate),
        UrlInformation = records,
      };
    }

    //

    private readonly Repository repository;
    private readonly UniqueIdGenerator uniqueIdGenerator;
    private readonly DateService dateService;

    private IEnumerable<UrlInfo> GetRecordsInRange(DateTime? fromDate, DateTime? toDate)
    {
      // do NOT call .ToList() - this MUST be re-evaluated
      var start = fromDate ?? new DateTime(1900, 1, 1);
      var end = toDate ?? new DateTime(2100, 1, 1);

      // if the end time is not specified, use the last second of the day
      if (end.TimeOfDay == new TimeSpan(0, 0, 0))
        end = end.AddDays(1).AddSeconds(-1);

      return repository
        .GetAll()
        .Where(it => it.ExpiresAt >= start && it.ExpiresAt <= end);
    }

    private static int FixPage(int? page, int pageCount)
    {
      page = page ?? 1; // page is 1-based
      if (page < 1)
        page = 1;
      if (page > pageCount)
        page = pageCount;

      return page.GetValueOrDefault();
    }
  }
}