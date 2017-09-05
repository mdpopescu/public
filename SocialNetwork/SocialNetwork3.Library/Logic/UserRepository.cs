using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork3.Library.Logic
{
    public class UserRepository
    {
        /// <summary>Adds a follower to the given user.</summary>
        /// <param name="follower">The follower.</param>
        /// <param name="followed">The user being followed.</param>
        public void AddFollower(string follower, string followed)
        {
            // does not allow a user to follow himself
            if (string.Equals(follower, followed, StringComparison.OrdinalIgnoreCase))
                return;

            if (!followers.ContainsKey(followed))
                followers.Add(followed, new List<string>());

            var list = followers[followed];
            if (!list.Contains(follower, StringComparer.OrdinalIgnoreCase))
                list.Add(follower);
        }

        /// <summary>Gets the followers of the given user.</summary>
        /// <param name="user">The user.</param>
        /// <returns>The list of followers.</returns>
        public IEnumerable<string> GetFollowers(string user) => followers.ContainsKey(user) ? followers[user] : Enumerable.Empty<string>();

        //

        private readonly Dictionary<string, List<string>> followers = new Dictionary<string, List<string>>();
    }
}