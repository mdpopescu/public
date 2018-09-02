using System;
using System.Linq;
using System.Windows.Forms;
using ExtractLinks.Contracts;
using ExtractLinks.Models;

namespace ExtractLinks.Services
{
    public class MainLogic
    {
        public MainLogic(MainUI ui)
        {
            this.ui = ui;
        }

        public void LoadLinksFrom(string url)
        {
            var page = WebPage.Load(url);
            var links = page.GetLinks();
            ui.SetLinks(links);
        }

        public void CopySelected()
        {
            var selected = ui.SelectedLinks.Select(lv => lv.Url);
            Clipboard.SetText(string.Join(Environment.NewLine, selected));
        }

        public void SelectMatching(string text)
        {
            var indices = ui
                .AllLinks
                .Select((link, index) => new { link, index })
                .Where(it => it.link.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(it => it.index)
                .ToList();
            ui.SelectLinks(indices);
        }

        //

        private readonly MainUI ui;
    }
}