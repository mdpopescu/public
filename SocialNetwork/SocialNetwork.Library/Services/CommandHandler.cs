using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;
using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Services
{
  public class CommandHandler : API
  {
    public CommandHandler(MessageRepository messages, UserRepository users, TimeFormatter timeFormatter)
    {
      this.messages = messages;
      this.users = users;
      this.timeFormatter = timeFormatter;
    }

    public void Post(string user, string message)
    {
      messages.Add(new Message(Sys.Time(), user, message));
    }

    public IEnumerable<string> Read(string user)
    {
      var time = Sys.Time();

      return messages
        .Get()
        .Where(it => it.User == user)
        .OrderByDescending(it => it.CreatedOn)
        .Select(it => it.Text + Prettify(time - it.CreatedOn));
    }

    public void Follow(string user, string other)
    {
      users.AddFollower(user, other);
    }

    public IEnumerable<string> Wall(string user)
    {
      return Enumerable.Empty<string>();
    }

    //

    private readonly MessageRepository messages;
    private readonly UserRepository users;
    private readonly TimeFormatter timeFormatter;

    private string Prettify(TimeSpan timeSpan)
    {
      return " (" + timeFormatter.Format(timeSpan) + ")";
    }
  }
}