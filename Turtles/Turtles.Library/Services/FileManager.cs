using Turtles.Library.Contracts;

namespace Turtles.Library.Services;

public class FileManager : IFileManager
{
    public string Text
    {
        get => text;
        set
        {
            if (text == value)
                return;

            text = value;
            IsModified = true;
        }
    }

    public string? Filename { get; private set; }
    public bool IsModified { get; private set; }

    public FileManager(IFileUI ui, IFileSystem fs)
    {
        this.ui = ui;
        this.fs = fs;
    }

    public bool New()
    {
        if (IsModified && !Save())
            return false;

        Text = "";
        Filename = null;
        IsModified = false;
        return true;
    }

    public bool Open()
    {
        if (IsModified && !Save())
            return false;

        var filename = ui.GetFilenameToOpen();
        return filename is not null && Try(filename, () => Text = fs.Load(filename));
    }

    public bool Save() => InternalSave(Filename);
    public bool SaveAs() => InternalSave(null);
    public bool SaveIfModified() => !IsModified || Save();

    //

    private readonly IFileUI ui;
    private readonly IFileSystem fs;

    private string text = "";

    private bool InternalSave(string? oldFilename)
    {
        var filename = oldFilename ?? ui.GetFilenameToSave();
        return filename is not null && Try(filename, () => fs.Save(filename, text));
    }

    private bool Try(string filename, Action action)
    {
        try
        {
            action.Invoke();
            Filename = filename;
            IsModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}