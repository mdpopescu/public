namespace Elomen.Library.Model
{
    public class Message
    {
        public string AccountId { get; private set; }
        public string Text { get; private set; }

        public Message(string accountId, string text)
        {
            AccountId = accountId;
            Text = text;
        }
    }
}