namespace Renfield.AppendOnly.Library
{
  public interface SerializationEngine
  {
    byte[] Serialize<T>(T value);
    T Deserialize<T>(byte[] buffer);
  }
}