using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Giles.Library.Models;

namespace Giles.Tests.Helper
{
  public class ApiOverHttp : Api
  {
    public ApiOverHttp(string baseUrl)
    {
      this.baseUrl = baseUrl;
    }

    public void Login(string user, string pass)
    {
      using (var web = new WebClient())
      {
        web.Headers["ContentType"] = "application/x-www-form-urlencoded; charset=UTF-8";

        var pairs = new[]
        {
          new KeyValuePair<string, string>("user", user),
          new KeyValuePair<string, string>("pass", pass),
        };
        token = web.UploadString(baseUrl + "/login", MakePostData(pairs));
      }
    }

    public string CreateEntry(string subject, string body, string[] labels)
    {
      using (var web = new WebClient())
      {
        web.Headers["ContentType"] = "application/x-www-form-urlencoded; charset=UTF-8";

        var pairs = labels
          .Select(it => new KeyValuePair<string, string>("label", it))
          .Concat(new[]
          {
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("body", body),
          });
        return web.UploadString(baseUrl + "/create", MakePostData(pairs));
      }
    }

    public Entry GetEntry(string id)
    {
      return new Entry("a", "b", new[] { "c" });

      //using (var web = new WebClient())
      //{
      //  return web.DownloadString(baseUrl + "/entry/" + id);
      //}
    }

    //

    private readonly string baseUrl;

    private string token;

    private static string MakePostData(IEnumerable<KeyValuePair<string, string>> pairs)
    {
      return string.Join("&", pairs.Select(it => it.Key + "=" + HttpUtility.UrlEncode(it.Value)));
    }
  }
}