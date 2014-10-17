namespace EventStore.Library.Contracts
{
  public interface Processor<in TIn>
  {
    void Process(TIn input);
  }
}