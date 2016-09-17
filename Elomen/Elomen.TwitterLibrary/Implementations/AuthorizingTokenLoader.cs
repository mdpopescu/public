using CoreTweet;
using Elomen.Storage.Contracts;
using Elomen.TwitterLibrary.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class AuthorizingTokenLoader : Loadable<Tokens>
    {
        public AuthorizingTokenLoader(Loadable<Tokens> loader, CompositeSettings appSettings, ResourceStore<CompositeSettings> userStore,
            Authorizable authorizer)
        {
            this.loader = loader;
            this.appSettings = appSettings;
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
                var tokens = authorizer.Authorize(appSettings["ConsumerKey"], appSettings["ConsumerSecret"]);

                SaveUserSettings(tokens);
                return tokens;
            }
        }

        //

        private readonly Loadable<Tokens> loader;
        private readonly CompositeSettings appSettings;
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