using System;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.oAuth;
using TweetNicer.Library.Contracts;

namespace TweetNicer.Library.Services
{
  public class TweetInviAuthenticator : Authenticator
  {
    public TweetInviAuthenticator(Func<WebServer> serverFunc, Func<IOAuthCredentials, TweetApi> apiFunc, string apiKey, string apiSecret)
    {
      this.serverFunc = serverFunc;
      this.apiFunc = apiFunc;
      this.apiKey = apiKey;
      this.apiSecret = apiSecret;
    }

    public TweetApi Authenticate()
    {
      using (var server = serverFunc())
      {
        var temporaryCredentials = CredentialsCreator.GenerateApplicationCredentials(apiKey, apiSecret);
        var url = CredentialsCreator.GetAuthorizationURLForCallback(temporaryCredentials, server.URL);
        var callbackURL = server.CallbackURL;
        var newCredentials = CredentialsCreator.GetCredentialsFromCallbackURL(callbackURL, temporaryCredentials);
        return apiFunc(newCredentials);
      }
    }

    //

    private readonly Func<WebServer> serverFunc;
    private readonly Func<IOAuthCredentials, TweetApi> apiFunc;
    private readonly string apiKey;
    private readonly string apiSecret;
  }
}