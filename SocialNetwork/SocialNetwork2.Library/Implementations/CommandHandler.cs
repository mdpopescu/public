using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork2.Library.Implementations
{
    public class CommandHandler
    {
        public CommandHandler(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IEnumerable<string> Handle(string command)
        {
            var parts = command.Split(' ');

            var userName = parts[0];
            var user = userRepository.CreateOrFind(userName);

            if (parts.Length == 1)
            {
                return user.Read();
            }
            if (parts[1] == "->")
            {
                user.Post(string.Join(" ", parts.Skip(2)));
                return Enumerable.Empty<string>();
            }
            if (parts[1] == "wall")
            {
                return user.Wall();
            }
            if (parts[1] == "follows")
            {
                var otherUserName = parts[2];
                var otherUser = userRepository.CreateOrFind(otherUserName);

                user.Follow(otherUser);
                return Enumerable.Empty<string>();
            }

            return Enumerable.Empty<string>();
        }

        //

        private readonly UserRepository userRepository;
    }
}