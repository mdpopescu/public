using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using ExtractLinks.Models;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ExtractLinks
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            clbLinks.DisplayMember = "Title";
            clbLinks.ValueMember = "Url";
        }

        //

        private int ttIndex;

        private static LinkView ToLinkView(HtmlNode it) =>
            new LinkView(it.InnerText, it.GetAttributeValue("href", ""));

        private static string LoadPage(string url)
        {
            using (var web = new WebClient())
                return web.DownloadString(url);
        }

        private static IEnumerable<LinkView> GetLinks(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc
                .DocumentNode
                .Descendants("a")
                .Select(ToLinkView)
                .Where(it => it.IsAbsolute && !it.IsAnonymous)
                .ToList();
        }

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        private void ShowLinks(IEnumerable<LinkView> links)
        {
            clbLinks.Items.Clear();
            clbLinks.Items.AddRange(links.ToArray());
        }

        private void ShowToolTip()
        {
            ttIndex = clbLinks.IndexFromPoint(clbLinks.PointToClient(MousePosition));
            if (ttIndex >= 0)
                toolTip1.SetToolTip(clbLinks, ((LinkView) clbLinks.Items[ttIndex]).Url);
        }

        //

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var page = LoadPage(txtUrl.Text);
            var links = GetLinks(page);
            ShowLinks(links);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            //
        }

        private void clbLinks_MouseHover(object sender, EventArgs e)
        {
            ShowToolTip();
        }

        private void clbLinks_MouseMove(object sender, MouseEventArgs e)
        {
            if (ttIndex != clbLinks.IndexFromPoint(e.Location))
                ShowToolTip();
        }
    }
}