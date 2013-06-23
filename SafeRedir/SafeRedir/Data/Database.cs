using System.Data.Entity;

namespace Renfield.SafeRedir.Data
{
  public class Database : DbContext, Repository
  {
    public IDbSet<UrlInfo> UrlInformation { get; set; }
  }
}