namespace Renfield.Licensing.Library.Contracts
{
  public interface Encryptor
  {
    string Encrypt(string s);
    string Decrypt(string s);
  }
}