using CoreTweet;
using Elomen.Storage.Contracts;

namespace Elomen.TwitterLibrary.Contracts
{
    public interface Authorizable
    {
        Tokens Authorize(CompositeSettings appSettings);
    }
}