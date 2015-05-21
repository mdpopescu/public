using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork.Library.Contracts;
using SocialNetwork.Library.Models;

namespace SocialNetwork.Library.Services
{
  public class CommandHandler : API
  {
    public CommandHandler(MessageRepository messages, UserRepository users)
    {
      this.messages = messages;
      this.users = users;
    }

    public void Post(string user, string message)
    {
      messages.Add(new Message(Sys.Time(), user, message));
    }

    public IEnumerable<string> Read(string user)
    {
      return GetUserMessages(user)
        .OrderByDescending(it => it.CreatedOn)
        .Select(FormattedMessage);
    }

    public void Follow(string user, string other)
    {
      users.AddFollower(user, other);
    }

    public IEnumerable<string> Wall(string user)
    {
      var list = users.GetFollowers(user).ToList();
      list.Insert(0, user);

      return from u in list
             from message in GetUserMessages(u)
             orderby message.CreatedOn descending
             select string.Format("{0} - {1}", u, FormattedMessage(message));
    }

    //

    private readonly MessageRepository messages;
    private readonly UserRepository users;

    private IEnumerable<Message> GetUserMessages(string user)
    {
      return messages
        .Get()
        .Where(it => it.User == user);
    }

    private string FormattedMessage(Message message)
    {
      return message.Text + Prettify(Sys.Time() - message.CreatedOn);
    }

    private string Prettify(TimeSpan timeSpan)
    {
      return " (" + TimeFormatter.Format(timeSpan) + ")";
    }
  }
}