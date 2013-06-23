using System;

namespace Renfield.SafeRedir.Services
{
  public class UniqueIdGeneratorFromGuid : UniqueIdGenerator
  {
    public string Generate()
    {
      var uniqueValue = GetUniqueValue();
      var bytes = BitConverter.GetBytes(uniqueValue);

      var base64 = Convert.ToBase64String(bytes);

      return base64
        .Replace('+', '-')
        .Replace('/', '_')
        .Replace("=", "");
    }

    //

    private static UInt64 GetUniqueValue()
    {
      var guid = Guid.NewGuid();
      var bytes = guid.ToByteArray();
      var values = new[]
      {
        BitConverter.ToUInt64(bytes, 0),
        BitConverter.ToUInt64(bytes, 8),
      };

      return values[0] ^ values[1];
    }
  }
}