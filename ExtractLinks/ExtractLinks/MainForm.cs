using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using ExtractLinks.Contracts;
using ExtractLinks.Models;
using ExtractLinks.Services;

namespace ExtractLinks
{
    public partial class MainForm : Form, MainUI
    {
        public IEnumerable<LinkView> SelectedLinks => clbLinks.CheckedItems.OfType<LinkView>();

        public MainForm()
        {
            InitializeComponent();

            logic = new MainLogic(this);

            clbLinks.DisplayMember = "Title";
            clbLinks.ValueMember = "Url";
        }

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public void SetLinks(IEnumerable<LinkView> links)
        {
            clbLinks.Items.Clear();
            clbLinks.Items.AddRange(links.ToArray());
        }

        //

        private readonly MainLogic logic;

        private int ttIndex;

        private void ShowToolTip()
        {
            ttIndex = clbLinks.IndexFromPoint(clbLinks.PointToClient(MousePosition));
            var caption = ttIndex >= 0 ? ((LinkView) clbLinks.Items[ttIndex]).Url : "";
            toolTip1.SetToolTip(clbLinks, caption);
        }

        //

        private void btnLoad_Click(object sender, EventArgs e)
        {
            logic.LoadLinksFrom(txtUrl.Text);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            logic.CopySelected();
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