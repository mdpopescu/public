namespace ExtractLinks.Models
{
    public class LinkView
    {
        public string Title { get; }
        public string Url { get; }

        public LinkView(string title, string url)
        {
            Title = title;
            Url = url;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}