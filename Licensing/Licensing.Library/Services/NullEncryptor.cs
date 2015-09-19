using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  /// <summary>
  ///   Implements the Null Object pattern for the Encryptor interface.
  /// </summary>
  public class NullEncryptor : Encryptor
  {
    public string Encrypt(string s)
    {
      return s + "";
    }

    public string Decrypt(string s)
    {
      return s + "";
    }
  }
}