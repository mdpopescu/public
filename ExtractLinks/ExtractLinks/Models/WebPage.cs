using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace ExtractLinks.Models
{
    public class WebPage
    {
        public static WebPage Load(string url)
        {
            using (var web = new WebClient())
                return new WebPage(web.DownloadString(url));
        }

        //

        public IEnumerable<LinkView> GetLinks()
        {
            return doc
                .DocumentNode
                .Descendants("a")
                .Select(ToLinkView)
                .Where(it => it.IsUseful)
                .ToList();
        }

        //

        private readonly HtmlDocument doc = new HtmlDocument();

        private static LinkView ToLinkView(HtmlNode it) =>
            new LinkView(it.InnerText, it.GetAttributeValue("href", ""));

        private WebPage(string html)
        {
            doc.LoadHtml(html);
        }
    }
}