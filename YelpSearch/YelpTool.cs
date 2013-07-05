using System;
using System.Net;

namespace Renfield.YelpSearch
{
  public class YelpTool
  {
    public YelpTool(YelpSettings settings)
    {
      this.settings = settings;
    }

    public string Search(string term, string location, int radiusInMiles)
    {
      radiusInMiles = NormalizeRadiusInMiles(radiusInMiles);

      var oauth = new OAuthBase();

      var query = new Uri(String.Format("http://api.yelp.com/v2/search?term={0}&location={1}&radius_filter={2}",
        Uri.EscapeDataString(term), Uri.EscapeDataString(location), ConvertToMeters(radiusInMiles)));

      string url;
      string parameters;
      var signature = oauth.GenerateSignature(query, settings.ConsumerKey, settings.ConsumerSecret, settings.Token, settings.TokenSecret, "GET",
        oauth.GenerateTimeStamp(), oauth.GenerateNonce(), OAuthBase.SignatureTypes.HMACSHA1, out url, out parameters);

      using (var web = new WebClient())
      {
        var newUrl = String.Format("{0}?{1}&oauth_signature={2}", url, parameters, Uri.EscapeDataString(signature));
        return web.DownloadString(newUrl);
      }
    }

    //

    private readonly YelpSettings settings;

    private static int NormalizeRadiusInMiles(int radiusInMiles)
    {
      if (radiusInMiles < 1)
        radiusInMiles = 1;
      else if (radiusInMiles > 25)
        radiusInMiles = 25;

      return radiusInMiles;
    }

    private static string ConvertToMeters(int radiusInMiles)
    {
      return Math.Round(radiusInMiles * 1609.3).ToString();
    }
  }
}