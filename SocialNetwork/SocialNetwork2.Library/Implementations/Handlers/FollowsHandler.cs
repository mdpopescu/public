using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations.Handlers
{
    public class FollowsHandler : IHandler
    {
        public FollowsHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public string KnownCommand => "follows";

        public IEnumerable<string> Handle(IUser user, string restOfInput)
        {
            var otherUser = userRepository.CreateOrFind(restOfInput);

            user.Follow(otherUser);
            return Enumerable.Empty<string>();
        }

        //

        private readonly IUserRepository userRepository;
    }
}