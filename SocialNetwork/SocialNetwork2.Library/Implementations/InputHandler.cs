using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations
{
    public class InputHandler
    {
        public InputHandler(IUserRepository userRepository, IEnumerable<IHandler> handlers)
        {
            this.userRepository = userRepository;
            this.handlers = handlers;
        }

        public IEnumerable<string> Handle(string input)
        {
            var parts = input.Split(' ');

            var userName = parts[0];
            var user = userRepository.CreateOrFind(userName);

            var command = parts.Length == 1 ? "" : parts[1];
            switch (command)
            {
                case "":
                    return user.Read();

                case "->":
                    user.Post(string.Join(" ", parts.Skip(2)));
                    return Enumerable.Empty<string>();

                case "wall":
                    return user.Wall();

                case "follows":
                    var otherUserName = parts[2];
                    var otherUser = userRepository.CreateOrFind(otherUserName);

                    user.Follow(otherUser);
                    return Enumerable.Empty<string>();
            }

            return Enumerable.Empty<string>();
        }

        //

        private readonly IUserRepository userRepository;
        private IEnumerable<IHandler> handlers;
    }
}