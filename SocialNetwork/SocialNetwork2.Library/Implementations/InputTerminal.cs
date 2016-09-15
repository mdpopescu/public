using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Library.Implementations
{
    public class InputTerminal
    {
        public InputTerminal(IUserRepository userRepository, IHandlerFactory handlerFactory)
        {
            this.userRepository = userRepository;
            this.handlerFactory = handlerFactory;
        }

        public IEnumerable<string> Handle(string input)
        {
            var parsed = Parse(input);
            var handler = handlerFactory.GetHandler(parsed.Command);
            return handler.Handle(parsed.User, parsed.RestOfInput);
        }

        //

        private readonly IUserRepository userRepository;
        private readonly IHandlerFactory handlerFactory;

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