using System;

namespace SocialNetwork.Library.Models
{
  public class Message
  {
    public DateTime CreatedOn { get; private set; }
    public string User { get; private set; }
    public string Text { get; private set; }

    public Message(DateTime createdOn, string user, string text)
    {
      CreatedOn = createdOn;
      User = user;
      Text = text;
    }
  }
}