using CoreTweet;
using Elomen.Storage.Contracts;
using Elomen.TwitterLibrary.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class AuthorizingTokenLoader : Loadable<Tokens>
    {
        public AuthorizingTokenLoader(Loadable<Tokens> loader, Loadable<CompositeSettings> appStore, ResourceStore<CompositeSettings> userStore,
            Authorizable authorizer)
        {
            this.loader = loader;
            this.appStore = appStore;
            this.userStore = userStore;
            this.authorizer = authorizer;
        }

        public Tokens Load()
        {
            try
            {
                return loader.Load();
            }
            catch
            {
                var settings = appStore.Load();
                var tokens = authorizer.Authorize(settings["ConsumerKey"], settings["ConsumerSecret"]);

                SaveUserSettings(tokens);
                return tokens;
            }
        }

        //

        private readonly Loadable<Tokens> loader;
        private readonly Loadable<CompositeSettings> appStore;
        private readonly ResourceStore<CompositeSettings> userStore;
        private readonly Authorizable authorizer;

        private void SaveUserSettings(Tokens tokens)
        {
            var settings = userStore.Load();

            settings["AccessToken"] = tokens.AccessToken;
            settings["AccessTokenSecret"] = tokens.AccessTokenSecret;

            userStore.Save(settings);
        }
    }
}