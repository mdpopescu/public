using System.Diagnostics;
using System.IO;

namespace Renfield.HotFolderWindowsService
{
  public class Reporter
  {
    public Reporter(EventLog log)
    {
      this.log = log;
    }

    public void Created(object sender, FileSystemEventArgs e)
    {
      log.WriteEntry("Created: " + e.FullPath);
    }

    public void Deleted(object sender, FileSystemEventArgs e)
    {
      log.WriteEntry("Deleted: " + e.FullPath);
    }

    public void Changed(object sender, FileSystemEventArgs e)
    {
      log.WriteEntry("Changed: " + e.FullPath);
    }

    public void Renamed(object sender, FileSystemEventArgs e)
    {
      log.WriteEntry("Renamed: " + e.FullPath);
    }

    //

    private readonly EventLog log;
  }
}