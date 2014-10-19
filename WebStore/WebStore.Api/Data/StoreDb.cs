using System.Data.Entity;

namespace WebStore.Api.Data
{
  public class StoreDb : DbContext
  {
    public DbSet<EventData> Events { get; set; }

    public StoreDb() : base("StoreDb")
    {
    }
  }
}