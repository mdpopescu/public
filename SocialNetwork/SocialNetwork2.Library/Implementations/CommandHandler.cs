using System;
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
            var index = command.IndexOf("->", StringComparison.Ordinal);
            if (index > 0)
            {
                var userName = command.Substring(0, index - 1);
                var user = userRepository.CreateOrFind(userName);
                user.Post(command.Substring(index + 3));

                return Enumerable.Empty<string>();
            }

            index = command.IndexOf("wall", StringComparison.OrdinalIgnoreCase);
            if (index >= 0 && index == command.Length - 4)
            {
                var userName = command.Substring(0, index - 1);
                var user = userRepository.CreateOrFind(userName);
                return user.Wall();
            }
            else
            {
                var user = userRepository.CreateOrFind(command);
                return user.Read();
            }
        }

        //

        private readonly UserRepository userRepository;
    }
}