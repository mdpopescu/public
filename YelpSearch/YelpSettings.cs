using System.Configuration;

namespace Renfield.YelpSearch
{
  public class YelpSettings
  {
    public string BaseUrl { get; private set; }
    public string ConsumerKey { get; private set; }
    public string ConsumerSecret { get; private set; }
    public string Token { get; private set; }
    public string TokenSecret { get; private set; }

    public YelpSettings()
    {
      var appSettings = ConfigurationManager.AppSettings;

      BaseUrl = appSettings["yelp_base_url"];
      ConsumerKey = appSettings["yelp_consumer_key"];
      ConsumerSecret = appSettings["yelp_consumer_secret"];
      Token = appSettings["yelp_token"];
      TokenSecret = appSettings["yelp_token_secret"];
    }
  }
}