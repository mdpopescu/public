using System.Data.Entity;

namespace Renfield.SafeRedir.Data
{
  public interface Repository
  {
    IDbSet<UrlInfo> UrlInformation { get; }

    int SaveChanges();
  }
}