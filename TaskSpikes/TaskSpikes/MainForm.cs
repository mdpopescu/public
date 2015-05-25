using System;
using System.Threading;
using System.Windows.Forms;

namespace TaskSpikes
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      runner = new TaskRunner();
    }

    //

    private readonly TaskRunner runner;

    private void LongRunningMethod(CancellationToken token)
    {
      for (var i = 0; i < 100000; i++)
      {
        if (lbItems.Items.Count > 100)
          lbItems.Items.Clear();

        lbItems.Items.Add(i.ToString());
      }
    }

    //

    private void btnStart_Click(object sender, EventArgs e)
    {
      runner.Start(LongRunningMethod);

      btnStart.Enabled = false;
      btnCancel.Enabled = true;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      runner.Cancel();

      btnStart.Enabled = true;
      btnCancel.Enabled = false;
    }
  }
}