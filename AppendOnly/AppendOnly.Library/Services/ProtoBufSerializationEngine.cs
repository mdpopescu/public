using System.IO;
using ProtoBuf;
using Renfield.AppendOnly.Library.Contracts;

namespace Renfield.AppendOnly.Library.Services
{
  public class ProtoBufSerializationEngine : SerializationEngine
  {
    public byte[] Serialize<T>(T value)
    {
      using (var m = new MemoryStream())
      {
        Serializer.Serialize(m, value);
        return m.ToArray();
      }
    }

    public T Deserialize<T>(byte[] buffer)
    {
      using (var m = new MemoryStream(buffer))
      {
        return Serializer.Deserialize<T>(m);
      }
    }
  }
}