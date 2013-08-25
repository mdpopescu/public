using System;
using System.Windows.Forms;
using Renfield.Anagrams.Properties;

namespace Renfield.Anagrams
{
  public partial class AboutForm : Form
  {
    public AboutForm()
    {
      InitializeComponent();
    }

    private void AboutForm_Load(object sender, EventArgs e)
    {
      txtContributions.Text = Resources.ExternalResources;
      btnOK.Select();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
    }
  }
}