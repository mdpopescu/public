namespace ExtractLinks.Models
{
    public class LinkView
    {
        public string Title { get; }
        public string Url { get; }

        public bool IsAbsolute { get; }
        public bool IsAnonymous { get; }

        public LinkView(string title, string url)
        {
            Title = title;
            Url = url;

            IsAbsolute = Url.StartsWith("http://") || Url.StartsWith("https://");
            IsAnonymous = string.IsNullOrWhiteSpace(Title);
        }
    }
}