using System.Collections.Generic;
using TweetNicer.Library.Model;

namespace TweetNicer.Library.Contracts
{
  public interface TweetApi
  {
    IEnumerable<TweetObject> GetTweets();
  }
}