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

      runner = new TaskRunner<Void>(LongRunningMethod);
    }

    //

    private readonly TaskRunner<Void> runner;

    private Void LongRunningMethod(CancellationToken token, object state)
    {
      this.UIChange(() => lbItems.Items.Clear());

      for (var i = 0; i < 100000; i++)
      {
        token.ThrowIfCancellationRequested();

        var s = i.ToString();
        this.UIChange(() =>
        {
          if (lbItems.Items.Count > 100)
            lbItems.Items.Clear();

          lbItems.Items.Add(s);
        });

        Thread.Sleep(100);
      }

      return Void.Singleton;
    }

    //

    private void btnStart_Click(object sender, EventArgs e)
    {
      runner.Start(null);

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