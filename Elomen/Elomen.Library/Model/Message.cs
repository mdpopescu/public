namespace Elomen.Library.Model
{
    public class Message
    {
        public Message Parent { get; set; }

        public Account Account { get; set; }
        public string Text { get; set; }
    }
}