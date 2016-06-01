using System.Collections.Generic;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations
{
    public class UserRepository : IUserRepository
    {
        public User CreateOrFind(string userName)
        {
            User user;
            if (!users.TryGetValue(userName, out user))
            {
                user = new User(userName);
                users.Add(userName, user);
            }

            return user;
        }

        //

        private readonly Dictionary<string, User> users = new Dictionary<string, User>();
    }
}