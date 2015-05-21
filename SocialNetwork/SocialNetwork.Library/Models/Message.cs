using System;
using SocialNetwork.Library.Services;

namespace SocialNetwork.Library.Models
{
  public class Message
  {
    public DateTime CreatedOn { get; private set; }
    public string User { get; private set; }
    public string Text { get; private set; }

    public Message(string user, string text)
    {
      CreatedOn = Sys.Time();
      User = user;
      Text = text;
    }
  }
}