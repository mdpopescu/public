using System;
using Renfield.SafeRedir.Data;

namespace Renfield.SafeRedir.Services
{
  public class DbShorteningService : ShorteningService
  {
    public DbShorteningService(Repository repository)
    {
      this.repository = repository;
    }

    public string CreateRedirect(string url, string safeUrl, int ttl)
    {
      var urlInfo = new UrlInfo
      {
        OriginalUrl = url,
        SafeUrl = safeUrl,
        ExpiresAt = SystemInfo.SystemClock().AddSeconds(ttl),
      };
      repository.UrlInformation.Add(urlInfo);
      repository.SaveChanges();

      return null;
    }

    //

    private readonly Repository repository;
  }
}