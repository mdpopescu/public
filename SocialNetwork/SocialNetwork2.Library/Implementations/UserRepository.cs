using System;
using System.Collections.Generic;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(Func<string, IUser> userFactory)
        {
            this.userFactory = userFactory;
        }

        public IUser CreateOrFind(string userName)
        {
            IUser user;
            if (!users.TryGetValue(userName, out user))
            {
                user = userFactory.Invoke(userName);
                users.Add(userName, user);
            }

            return user;
        }

        //

        private readonly Dictionary<string, IUser> users = new Dictionary<string, IUser>();

        private readonly Func<string, IUser> userFactory;
    }
}