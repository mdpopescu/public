namespace Elomen.Storage.Contracts
{
    /// <summary>
    /// Generic storage.
    /// </summary>
    public interface GenericStorage<TValue>
    {
        TValue UserValues { get; set; }
        TValue MachineValues { get; set; }
    }
}