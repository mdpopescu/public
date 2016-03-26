using System;
using System.Net;

namespace SocialClone.Tests
{
  // from http://stackoverflow.com/a/29479390
  public class CookieAwareWebClient : WebClient
  {
    public CookieContainer CookieContainer { get; }
    public CookieCollection ResponseCookies { get; set; }

    public CookieAwareWebClient()
    {
      CookieContainer = new CookieContainer();
      ResponseCookies = new CookieCollection();
    }

    protected override WebRequest GetWebRequest(Uri address)
    {
      var request = (HttpWebRequest) base.GetWebRequest(address);
      // ReSharper disable once PossibleNullReferenceException
      request.CookieContainer = CookieContainer;
      return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
      var response = (HttpWebResponse) base.GetWebResponse(request);
      // ReSharper disable once PossibleNullReferenceException
      ResponseCookies = response.Cookies;
      return response;
    }
  }
}