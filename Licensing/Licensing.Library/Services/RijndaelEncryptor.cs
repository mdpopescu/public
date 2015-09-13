using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class RijndaelEncryptor : Encryptor
  {
    public RijndaelEncryptor(string password, string salt)
    {
      var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

      key = CreateKey(deriveBytes);
    }

    public string Encrypt(string s)
    {
      var encryptor = CreateEncryptor();

      var plainTextBytes = Encoding.UTF8.GetBytes(s);
      var cipherTextBytes = Encrypt(plainTextBytes, encryptor);

      return Convert.ToBase64String(cipherTextBytes);
    }

    public string Decrypt(string s)
    {
      var decryptor = CreateDecryptor();

      var cipherTextBytes = Convert.FromBase64String(s);
      var plainTextBytes = Decrypt(cipherTextBytes, decryptor);

      return Encoding.UTF8.GetString(plainTextBytes);
    }

    //

    private readonly RijndaelManaged key;

    private static RijndaelManaged CreateKey(DeriveBytes deriveBytes)
    {
      var result = new RijndaelManaged {Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7};
      result.Key = deriveBytes.GetBytes(result.KeySize / 8);
      result.IV = deriveBytes.GetBytes(result.BlockSize / 8);

      return result;
    }

    private ICryptoTransform CreateEncryptor()
    {
      return key.CreateEncryptor(key.Key, key.IV);
    }

    private ICryptoTransform CreateDecryptor()
    {
      return key.CreateDecryptor(key.Key, key.IV);
    }

    private static byte[] Encrypt(byte[] bytes, ICryptoTransform encryptor)
    {
      using (var memoryStream = new MemoryStream())
      using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
      {
        cryptoStream.Write(bytes, 0, bytes.Length);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
      }
    }

    private static byte[] Decrypt(byte[] bytes, ICryptoTransform decryptor)
    {
      using (var memoryStream = new MemoryStream(bytes))
      using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
      {
        var plainTextBytes = new byte[bytes.Length];
        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

        return plainTextBytes.Take(decryptedByteCount).ToArray();
      }
    }
  }
}