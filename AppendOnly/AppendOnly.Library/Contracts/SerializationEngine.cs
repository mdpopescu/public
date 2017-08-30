namespace Renfield.AppendOnly.Library.Contracts
{
    public interface SerializationEngine
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] buffer);
    }
}