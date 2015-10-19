using Giles.Library.Models;

namespace Giles.Tests.Helper
{
  public interface Api
  {
    void Login(string user, string pass);
    string CreateEntry(string subject, string body, string[] labels);
    Entry GetEntry(string id);
  }
}