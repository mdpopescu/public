using System;

namespace Elomen.TwitterLibrary.Models
{
    public class TwitterMessage
    {
        public TwitterAccount Account { get; }
        public string Text { get; }

        public TwitterMessage(TwitterAccount account, string text)
        {
            if (account == null)
                throw new NullReferenceException(nameof(account));
            if (text == null)
                throw new NullReferenceException(nameof(text));

            Account = account;
            Text = text;
        }
    }
}