using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialClone.Library.Properties;

namespace SocialClone.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private const string BASE_URL = "http://localhost:5972";

    private const string TOKEN_NAME = "__RequestVerificationToken";

    [TestMethod]
    public void RegisteringNewUser()
    {
      var unique = Guid.NewGuid().ToString("N");

      string html;
      HtmlNode root;

      Cookie[] cookies;
      html = Get("Account/Register", out cookies);
      root = Parse(html);
      var formToken = GetFormToken(root);
      var cookieToken = GetCookieToken(cookies);

      html = Post("Account/Register", new
      {
        Email = unique + "@example.com",
        User = unique,
        Password = unique,
        ConfirmPassword = unique,
      },
        formToken, cookieToken);
      root = Parse(html);

      var message = root.SelectSingleNode("//*[@id='message']");
      Assert.IsNotNull(message);
      Assert.AreEqual(Resources.RegistrationMessage, message.InnerText);
    }

    //

    private static string Get(string url)
    {
      using (var web = new WebClient())
        return web.DownloadString(BASE_URL + "/" + url);
    }

    private static string Get(string url, out Cookie[] responseCookies)
    {
      using (var web = new CookieAwareWebClient())
      {
        var result = web.DownloadString(BASE_URL + "/" + url);
        responseCookies = web.ResponseCookies.Cast<Cookie>().ToArray();

        return result;
      }
    }

    private static string Post(string url, object body, string formToken = null, string cookieToken = null)
    {
      using (var web = new WebClient())
      {
        var data = GetPostData(body);

        web.Headers["Content-Type"] = "application/x-www-form-urlencoded";
        if (formToken != null)
          data += "&" + TOKEN_NAME + "=" + formToken;
        if (cookieToken != null)
          web.Headers.Add(HttpRequestHeader.Cookie, TOKEN_NAME + "=" + cookieToken);

        return web.UploadString(BASE_URL + "/" + url, data);
      }
    }

    private static string GetPostData(object obj)
    {
      var kv = obj
        .GetType()
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Select(prop => prop.Name + "=" + WebUtility.UrlEncode(prop.GetValue(obj) + ""))
        .ToArray();
      return string.Join("&", kv);
    }

    private static HtmlNode Parse(string html)
    {
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      Assert.IsTrue(doc.ParseErrors == null || !doc.ParseErrors.Any());
      Assert.IsNotNull(doc.DocumentNode);

      return doc.DocumentNode;
    }

    private static string GetFormToken(HtmlNode root)
    {
      var formToken = root.SelectSingleNode("//*[@name='" + TOKEN_NAME + "']");
      Assert.IsNotNull(formToken);

      return formToken.GetAttributeValue("value", null);
    }

    private static string GetCookieToken(IEnumerable<Cookie> cookies)
    {
      var cookieToken = cookies.Where(it => it.Name == TOKEN_NAME).FirstOrDefault();
      Assert.IsNotNull(cookieToken);

      return cookieToken.Value;
    }
  }
}