namespace ExtractLinks.Models
{
    public class LinkView
    {
        public string Title { get; }
        public string Url { get; }

        public bool IsUseful
        {
            get
            {
                var isAbsolute = Url.StartsWith("http://") || Url.StartsWith("https://");
                var isAnonymous = string.IsNullOrWhiteSpace(Title);
                return isAbsolute && !isAnonymous;
            }
        }

        public LinkView(string title, string url)
        {
            Title = title;
            Url = url;
        }
    }
}