using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
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