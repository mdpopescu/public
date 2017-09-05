using System.Collections.Generic;
using System.Linq;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Logic
{
    public class MessageRepository
    {
        public void Add(Message message)
        {
            //
        }

        public IEnumerable<Message> GetMessagesBy(string user)
        {
            return Enumerable.Empty<Message>();
        }
    }
}