using Telerik.WinControls;
using Telerik.WinControls.UI;
using Turtles.Library.Contracts;
using Turtles.Library.Models;
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
        dlg.Filter = Constants.FILTER;
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
    }

    public string? GetFilenameToSave()
    {
        using var dlg = new SaveFileDialog();
        dlg.Filter = Constants.FILTER;
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
    }

    public ConfirmationResponse ConfirmSave()
    {
        var result = RadMessageBox.Show(Constants.SAVE_PROMPT, Constants.SAVE_CAPTION, MessageBoxButtons.YesNoCancel);
        return result switch
        {
            DialogResult.Yes => ConfirmationResponse.YES,
            DialogResult.No => ConfirmationResponse.NO,
            _ => ConfirmationResponse.CANCEL,
        };
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

    private string Prefix => fileManager.IsModified ? Constants.MODIFIED_PREFIX : Constants.UNMODIFIED_PREFIX;
    private string Title => $"{Constants.APP_NAME} -- {Prefix}{fileManager.Filename ?? Constants.UNNAMED}";

    private void UpdateUI()
    {
        rrtxtCode.Text = fileManager.Text;
        Text = Title;
    }

    //

    private void rmiFileNew_Click(object sender, EventArgs e)
    {
        fileManager.New();
        UpdateUI();
    }

    private void rmiFileOpen_Click(object sender, EventArgs e)
    {
        fileManager.Open();
        UpdateUI();
    }

    private void rmiFileSave_Click(object sender, EventArgs e)
    {
        fileManager.Save();
        UpdateUI();
    }

    private void rmiFileSaveAs_Click(object sender, EventArgs e)
    {
        fileManager.SaveAs();
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