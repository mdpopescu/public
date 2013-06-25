using System.Collections.Generic;

namespace Renfield.SafeRedir.Data
{
  public interface Repository
  {
    int SaveChanges();

    void AddUrlInfo(UrlInfo urlInfo);
    UrlInfo GetUrlInfo(string id);
    IEnumerable<UrlInfo> GetAll();
  }
}