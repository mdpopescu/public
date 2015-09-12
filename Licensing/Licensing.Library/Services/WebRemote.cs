using System.Net;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class WebRemote : Remote
  {
    public string Get(string address)
    {
      using (var web = new WebClient())
      {
        return web.DownloadString(address);
      }
    }
  }
}