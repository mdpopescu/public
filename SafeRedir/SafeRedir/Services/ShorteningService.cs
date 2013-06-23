namespace Renfield.SafeRedir.Services
{
  public interface ShorteningService
  {
    string Shorten(string url, string safeUrl, int ttl);
  }
}