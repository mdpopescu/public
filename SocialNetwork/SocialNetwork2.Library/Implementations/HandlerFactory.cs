using System;
using System.Collections.Generic;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations
{
    public class HandlerFactory : IHandlerFactory
    {
        public HandlerFactory(IUserRepository userRepository)
        {
            handlers = new Dictionary<string, IHandler>(StringComparer.OrdinalIgnoreCase)
            {
                { "", new ReadHandler() },
                { "->", new PostHandler() },
                { "follows", new FollowsHandler(userRepository) },
                { "wall", new WallHandler() }
            };
        }

        public IHandler GetHandler(string command)
        {
            return handlers.ContainsKey(command) ? handlers[command] : NullHandler.INSTANCE;
        }

        //

        private readonly Dictionary<string, IHandler> handlers;
    }
}