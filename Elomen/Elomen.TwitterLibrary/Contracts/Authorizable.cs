using CoreTweet;

namespace Elomen.TwitterLibrary.Contracts
{
    public interface Authorizable
    {
        Tokens Authorize(string key, string secret);
    }
}