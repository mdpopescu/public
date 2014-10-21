using System.Security.Cryptography;

namespace FindDuplicates.Services
{
  public static class Utilities
  {
    public static byte[] GetHash(this byte[] bytes)
    {
      return md5.ComputeHash(bytes);
    }

    //

    private static readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
  }
}