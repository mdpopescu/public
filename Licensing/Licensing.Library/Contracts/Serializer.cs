namespace Renfield.Licensing.Library.Contracts
{
  public interface Serializer<T>
  {
    string Serialize(T obj);
    T Deserialize(string s);
  }
}