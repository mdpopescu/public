namespace Renfield.SafeRedir.Services
{
  public interface ShorteningService
  {
    string CreateRedirect(string url, string safeUrl, int ttl);
  }
}