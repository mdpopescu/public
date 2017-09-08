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

            if (!following.ContainsKey(follower))
                following.Add(follower, new List<string>());

            var list = following[follower];
            if (!list.Contains(followed, StringComparer.OrdinalIgnoreCase))
                list.Add(followed);
        }

        /// <summary>Returns the users being followed by the given user (including the given user).</summary>
        /// <param name="user">The user.</param>
        /// <returns>The list of users being followed.</returns>
        /// <remarks>The list being returned includes the given user.</remarks>
        public IEnumerable<string> GetFollowed(string user) => SafeGet(following, user).Concat(new[] { user });

        //

        private readonly Dictionary<string, List<string>> following = new Dictionary<string, List<string>>();

        private static IEnumerable<string> SafeGet(IReadOnlyDictionary<string, List<string>> dictionary, string user) =>
            dictionary.ContainsKey(user)
                ? dictionary[user]
                : Enumerable.Empty<string>();
    }
}