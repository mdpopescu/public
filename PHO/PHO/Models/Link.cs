using HtmlAgilityPack;

namespace PHO.Models
{
    public class Link : Element
    {
        public Link(string url)
            : base(null)
        {
            this.url = url;
        }

        public Element Load()
        {
            var web = new HtmlWeb
            {
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.137 Safari/537.36"
            };

            var doc = web.Load(url);
            return new Element(doc.DocumentNode);
        }

        //

        private readonly string url;
    }
}