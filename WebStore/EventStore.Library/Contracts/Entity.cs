namespace EventStore.Library.Contracts
{
  public interface Entity<out TKey>
  {
    TKey Id { get; }
  }
}