using Renfield.SafeRedir.Data;

namespace Renfield.SafeRedir.Services
{
  public class DbShorteningService : ShorteningService
  {
    public DbShorteningService(Repository repository, UniqueIdGenerator uniqueIdGenerator)
    {
      this.repository = repository;
      this.uniqueIdGenerator = uniqueIdGenerator;
    }

    public string CreateRedirect(string url, string safeUrl, int ttl)
    {
      var urlInfo = new UrlInfo
      {
        Id = uniqueIdGenerator.Generate(),
        OriginalUrl = url,
        SafeUrl = safeUrl,
        ExpiresAt = SystemInfo.SystemClock().AddSeconds(ttl),
      };
      repository.UrlInformation.Add(urlInfo);
      repository.SaveChanges();

      return urlInfo.Id;
    }

    //

    private readonly Repository repository;
    private readonly UniqueIdGenerator uniqueIdGenerator;
  }
}