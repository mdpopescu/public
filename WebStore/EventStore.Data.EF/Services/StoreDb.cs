using System.Data.Entity;
using EventStore.Data.EF.Models;

namespace EventStore.Data.EF.Services
{
  public class StoreDb : DbContext
  {
    public DbSet<EventData> Events { get; set; }

    public StoreDb() : base("StoreDb")
    {
    }
  }
}