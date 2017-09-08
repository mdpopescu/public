using System;
using System.Collections.Generic;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Library.Coordinators.Commands
{
    public class FollowsCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="FollowsCommand"/> class.</summary>
        /// <param name="users">The user repository.</param>
        public FollowsCommand(UserRepository users)
        {
            this.users = users;
        }

        public override List<string> Execute(DateTime time, string user, string argument)
        {
            users.AddFollower(user, argument);
            return new List<string>();
        }

        //

        private readonly UserRepository users;
    }
}