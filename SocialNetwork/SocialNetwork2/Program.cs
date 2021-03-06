﻿using System;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var userRepository = new UserRepository(name => new User(name));
            var commandHandler = new InputTerminal(userRepository, new HandlerFactory(userRepository));

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