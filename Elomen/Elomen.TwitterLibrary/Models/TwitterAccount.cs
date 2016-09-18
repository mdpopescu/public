using CoreTweet;

namespace Elomen.TwitterLibrary.Models
{
    public class TwitterAccount
    {
        public long Id { get; }
        public string Username { get; }

        public TwitterAccount(long id, string username)
        {
            Id = id;
            Username = username;
        }

        public TwitterAccount(User user)
            : this(user.Id.GetValueOrDefault(), user.ScreenName)
        {
        }
    }
}