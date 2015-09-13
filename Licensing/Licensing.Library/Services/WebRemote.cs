using System.Net;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class WebRemote : Remote
  {
    public WebRemote(string url)
    {
      this.url = url;
    }

    public string Get(string query)
    {
      using (var web = new WebClient())
      {
        return web.DownloadString(url + "?" + query);
      }
    }

    public string Post(string data)
    {
      return "";
    }

    //

    private readonly string url;
  }
}