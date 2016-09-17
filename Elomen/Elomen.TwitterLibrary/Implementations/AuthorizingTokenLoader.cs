using CoreTweet;
using Elomen.Storage.Contracts;
using Elomen.TwitterLibrary.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class AuthorizingTokenLoader : Loadable<Tokens>
    {
        public AuthorizingTokenLoader(Loadable<Tokens> loader, Authorizable authorizer, ResourceStore<CompositeSettings> userStore)
        {
            this.loader = loader;
            this.authorizer = authorizer;
            this.userStore = userStore;
        }

        public Tokens Load()
        {
            try
            {
                return loader.Load();
            }
            catch
            {
                var tokens = authorizer.Authorize();

                SaveUserSettings(tokens);
                return tokens;
            }
        }

        //

        private readonly Loadable<Tokens> loader;
        private readonly Authorizable authorizer;
        private readonly ResourceStore<CompositeSettings> userStore;

        private void SaveUserSettings(Tokens tokens)
        {
            var settings = userStore.Load();

            settings["AccessToken"] = tokens.AccessToken;
            settings["AccessTokenSecret"] = tokens.AccessTokenSecret;

            userStore.Save(settings);
        }
    }
}