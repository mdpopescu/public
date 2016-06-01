using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Library.Implementations
{
    public class InputTerminal
    {
        public InputTerminal(IUserRepository userRepository, IEnumerable<IHandler> handlers)
        {
            this.userRepository = userRepository;
            this.handlers = handlers;
        }

        public IEnumerable<string> Handle(string input)
        {
            var parsed = Parse(input);

            var result = handlers
                .Where(it => string.Compare(parsed.Command, it.KnownCommand, StringComparison.OrdinalIgnoreCase) == 0)
                .Select(it => it.Handle(parsed.User, parsed.RestOfInput))
                .FirstOrDefault();
            return result ?? Enumerable.Empty<string>();
        }

        //

        private readonly IUserRepository userRepository;
        private readonly IEnumerable<IHandler> handlers;

        private ParsedInput Parse(string input)
        {
            var parts = input.Split(' ');

            return new ParsedInput
            {
                User = userRepository.CreateOrFind(parts[0]),
                Command = parts.Length == 1 ? "" : parts[1],
                RestOfInput = string.Join(" ", parts.Skip(2)),
            };
        }
    }
}