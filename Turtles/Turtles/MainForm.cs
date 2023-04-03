using Telerik.WinControls.UI;
using Turtles.Library.Contracts;
using Turtles.Library.Services;

namespace Turtles;

public partial class MainForm : RadForm, IMainForm, IFileUI
{
    public MainForm()
    {
        InitializeComponent();

        app = new MainLogic(this);
        fileManager = new FileManager(this, new WinFS());
    }

    public string? GetFilenameToOpen()
    {
        using var dlg = new OpenFileDialog();
        dlg.Filter = "Logo Files (*.logo)|*.logo|All Files (*.*)|*.*";
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
    }

    public string? GetFilenameToSave()
    {
        using var dlg = new SaveFileDialog();
        dlg.Filter = "Logo Files (*.logo)|*.logo|All Files (*.*)|*.*";
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
    }

    //

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        app.Reset();
    }

    //

    private readonly MainLogic app;
    private readonly IFileManager fileManager;

    private void UpdateUI()
    {
        rrtxtCode.Text = fileManager.Text;
        var modified = fileManager.IsModified ? "*" : "";
        Text = $"Turtles -- {modified}{fileManager.Filename}";
    }

    //

    private void rmiFileNew_Click(object sender, EventArgs e)
    {
        if (fileManager.New())
            UpdateUI();
    }

    private void rmiFileOpen_Click(object sender, EventArgs e)
    {
        if (fileManager.Open())
            UpdateUI();
    }

    private void rmiFileSave_Click(object sender, EventArgs e)
    {
        if (fileManager.Save())
            UpdateUI();
    }

    private void rmiFileSaveAs_Click(object sender, EventArgs e)
    {
        if (fileManager.SaveAs())
            UpdateUI();
    }

    private void rmiFileExit_Click(object sender, EventArgs e)
    {
        if (fileManager.SaveIfModified())
            Application.Exit();
    }

    private void rrtxtCode_TextChanged(object sender, EventArgs e)
    {
        fileManager.Text = rrtxtCode.Text;
        UpdateUI();
    }

    private void rmiHelpAbout_Click(object sender, EventArgs e)
    {
        app.HelpAbout();
    }
}