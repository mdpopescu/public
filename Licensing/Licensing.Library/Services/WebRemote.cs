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
      using (var web = new WebClient())
      {
        web.Headers["Content-Type"] = "application/x-www-form-urlencoded";
        return web.UploadString(url, data);
      }
    }

    //

    private readonly string url;
  }
}