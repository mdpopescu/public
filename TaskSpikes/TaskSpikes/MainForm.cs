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
      //
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