using System;

namespace SecurePasswordStorage.Library.Models
{
    public class UserKey : GenericKey<string>
    {
        public UserKey(string value)
            : base(value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Invalid user key.", nameof(value));
        }
    }
}