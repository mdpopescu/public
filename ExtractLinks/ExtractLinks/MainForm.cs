using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using ExtractLinks.Models;

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
            var page = WebPage.Load(txtUrl.Text);
            var links = page.GetLinks();
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