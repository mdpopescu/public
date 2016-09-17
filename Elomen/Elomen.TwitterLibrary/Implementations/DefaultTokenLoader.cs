using CoreTweet;
using Elomen.Storage.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class DefaultTokenLoader : Loadable<Tokens>
    {
        public DefaultTokenLoader(ResourceStore<CompositeSettings> appStore, ResourceStore<CompositeSettings> userStore)
        {
            this.appStore = appStore;
            this.userStore = userStore;
        }

        public Tokens Load()
        {
            var appSettings = appStore.Load();
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

        private readonly ResourceStore<CompositeSettings> appStore;
        private readonly ResourceStore<CompositeSettings> userStore;
    }
}