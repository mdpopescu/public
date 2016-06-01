using System;
using System.Collections.Generic;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2
{
    class Program
    {
        private static void Main(string[] args)
        {
            var userRepository = new UserRepository(name => new User(name));
            var knownCommands = new List<IHandler>
            {
                new ReadHandler(),
                new PostHandler(),
                new FollowsHandler(userRepository),
                new WallHandler(),
            };
            var commandHandler = new InputTerminal(userRepository, knownCommands);

            Console.WriteLine("Social Network");
            Console.WriteLine();

            while (true)
            {
                Console.Write("> ");

                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;

                var output = commandHandler.Handle(input);
                foreach (var line in output)
                    Console.WriteLine(line);
            }
        }
    }
}