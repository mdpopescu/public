using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace PHO.Models
{
    public class Element
    {
        public string Text => doc.InnerText;

        public Element(HtmlNode doc)
        {
            this.doc = doc;
        }

        public Link ToLink()
        {
            return new Link(doc.Attributes["href"].Value);
        }

        public IEnumerable<Element> Each(string xpath)
        {
            return doc
                .SelectNodes(xpath)
                .Select(node => new Element(node));
        }

        //

        private readonly HtmlNode doc;
    }
}