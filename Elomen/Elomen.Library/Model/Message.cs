namespace Elomen.Library.Model
{
    public class Message
    {
        public Account Account { get; private set; }
        public string Text { get; private set; }

        public Message(Account account, string text)
        {
            Account = account;
            Text = text;
        }
    }
}