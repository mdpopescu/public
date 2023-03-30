using Telerik.WinControls.UI;
using Turtles.Library.Contracts;
using Turtles.Library.Services;

namespace Turtles;

public partial class MainForm : RadForm, IMainForm
{
    public MainForm()
    {
        InitializeComponent();

        app = new MainLogic(this);
    }

    //

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        app.Reset();
    }

    //

    private readonly MainLogic app;

    //

    private void rmiFileNew_Click(object sender, EventArgs e)
    {
        app.FileNew();
    }

    private void rmiFileOpen_Click(object sender, EventArgs e)
    {
        app.FileOpen();
    }

    private void rmiFileSave_Click(object sender, EventArgs e)
    {
        app.FileSave();
    }

    private void rmiFileSaveAs_Click(object sender, EventArgs e)
    {
        app.FileSaveAs();
    }

    private void rmiFileExit_Click(object sender, EventArgs e)
    {
        app.FileExit();
    }

    private void rmiHelpAbout_Click(object sender, EventArgs e)
    {
        app.HelpAbout();
    }
}