using CoreTweet;
using Elomen.Storage.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class DefaultTokenLoader : Loadable<Tokens>
    {
        public DefaultTokenLoader(CompositeSettings appSettings, CompositeSettings userSettings)
        {
            this.appSettings = appSettings;
            this.userSettings = userSettings;
        }

        public Tokens Load()
        {
            return new Tokens
            {
                ConsumerKey = appSettings["ConsumerKey"],
                ConsumerSecret = appSettings["ConsumerSecret"],
                AccessToken = userSettings["AccessToken"],
                AccessTokenSecret = userSettings["AccessTokenSecret"],
            };
        }

        //

        private readonly CompositeSettings appSettings;
        private readonly CompositeSettings userSettings;
    }
}