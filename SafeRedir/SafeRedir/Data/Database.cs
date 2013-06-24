using System.Data.Entity;
using System.Data.SqlServerCe;
using System.Linq;

namespace Renfield.SafeRedir.Data
{
  public class Database : DbContext, Repository
  {
    public DbSet<UrlInfo> UrlInformation { get; set; }

    public Database()
      : base(new SqlCeConnection("Data Source=Database.sdf; Persist Security Info=false;"), true)
    {
    }

    public void AddUrlInfo(UrlInfo urlInfo)
    {
      UrlInformation.Add(urlInfo);
    }

    public UrlInfo GetUrlInfo(string id)
    {
      return UrlInformation.FirstOrDefault(it => it.Id == id);
    }
  }
}