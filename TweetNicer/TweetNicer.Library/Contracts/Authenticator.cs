namespace TweetNicer.Library.Contracts
{
  public interface Authenticator
  {
    TweetApi Authenticate();
  }
}