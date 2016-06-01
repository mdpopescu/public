using System;
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

            string command, restOfInput;
            if (parts.Length == 1)
            {
                command = "";
                restOfInput = input;
            }
            else
            {
                command = parts[1];
                restOfInput = string.Join(" ", parts.Skip(2));
            }

            var result = handlers
                .Where(it => string.Compare(command, it.KnownCommand, StringComparison.OrdinalIgnoreCase) == 0)
                .Select(it => it.Handle(user, restOfInput))
                .FirstOrDefault();
            return result ?? Enumerable.Empty<string>();
        }

        //

        private readonly IUserRepository userRepository;
        private readonly IEnumerable<IHandler> handlers;
    }
}