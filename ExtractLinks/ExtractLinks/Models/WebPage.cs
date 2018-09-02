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
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc
                .DocumentNode
                .Descendants("a")
                .Select(ToLinkView)
                .Where(it => it.IsUseful)
                .ToList();
        }

        //

        private static LinkView ToLinkView(HtmlNode it) =>
            new LinkView(it.InnerText, it.GetAttributeValue("href", ""));

        private readonly string html;

        private WebPage(string html)
        {
            this.html = html;
        }
    }
}