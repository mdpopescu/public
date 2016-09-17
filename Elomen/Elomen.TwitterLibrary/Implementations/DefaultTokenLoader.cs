using CoreTweet;
using Elomen.Storage.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class DefaultTokenLoader : Loadable<Tokens>
    {
        public DefaultTokenLoader(CompositeSettings appSettings, Loadable<CompositeSettings> userStore)
        {
            this.appSettings = appSettings;
            this.userStore = userStore;
        }

        public Tokens Load()
        {
            var userSettings = userStore.Load();

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
        private readonly Loadable<CompositeSettings> userStore;
    }
}