using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class RijndaelEncryptor : Encryptor
  {
    public RijndaelEncryptor(string password)
    {
      // one time initialization
      symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};

      var deriveBytes = new Rfc2898DeriveBytes(password, SALT_SIZE / 8);

      keyBytes = deriveBytes.GetBytes(symmetricKey.KeySize / 8);
      initVectorBytes = deriveBytes.GetBytes(symmetricKey.BlockSize / 8);
    }

    public string Encrypt(string s)
    {
      var encryptor = CreateEncryptor();

      using (var memoryStream = new MemoryStream())
      using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
      {
        var plainTextBytes = Encoding.UTF8.GetBytes(s);

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();

        var cipherTextBytes = memoryStream.ToArray();

        return Convert.ToBase64String(cipherTextBytes);
      }
    }

    public string Decrypt(string s)
    {
      var decryptor = CreateDecryptor();

      var cipherTextBytes = Convert.FromBase64String(s);
      using (var memoryStream = new MemoryStream(cipherTextBytes))
      using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
      {
        var plainTextBytes = new byte[cipherTextBytes.Length];
        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
      }
    }

    //

    private const int SALT_SIZE = 256;

    private readonly RijndaelManaged symmetricKey;
    private readonly byte[] keyBytes;
    private readonly byte[] initVectorBytes;

    private ICryptoTransform CreateEncryptor()
    {
      return symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
    }

    private ICryptoTransform CreateDecryptor()
    {
      return symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
    }
  }
}