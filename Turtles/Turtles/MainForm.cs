using Telerik.WinControls.UI;

namespace Turtles;

public partial class MainForm : RadForm
{
    public MainForm()
    {
        InitializeComponent();
    }

    //

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Reset();
    }

    //

    private void Reset()
    {
        // TODO
    }

    //

    private void rmiFileOpen_Click(object sender, EventArgs e)
    {
        //
    }

    private void rmiFileSave_Click(object sender, EventArgs e)
    {
        //
    }

    private void rmiFileSaveAs_Click(object sender, EventArgs e)
    {
        //
    }

    private void rmiFileExit_Click(object sender, EventArgs e)
    {
        //
    }

    private void rmiHelpAbout_Click(object sender, EventArgs e)
    {
        //
    }
}