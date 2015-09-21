using System.Net;
using Renfield.Licensing.Library.Contracts;

namespace Renfield.Licensing.Library.Services
{
  public class WebRemote : Remote
  {
    public WebRemote(string submitUrl)
    {
      this.submitUrl = submitUrl;
    }

    public string Get(string query)
    {
      using (var web = new WebClient())
      {
        try
        {
          return web.DownloadString(query);
        }
        catch
        {
          return null;
        }
      }
    }

    public string Post(string data)
    {
      using (var web = new WebClient())
      {
        web.Headers["Content-Type"] = "application/x-www-form-urlencoded";
        return web.UploadString(submitUrl, data);
      }
    }

    //

    private readonly string submitUrl;
  }
}