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
      log.WriteEntry(string.Format("Created: {0}", e.FullPath), EventLogEntryType.Information);
    }

    public void Deleted(object sender, FileSystemEventArgs e)
    {
      log.WriteEntry(string.Format("Deleted: {0}", e.FullPath), EventLogEntryType.Information);
    }

    public void Changed(object sender, FileSystemEventArgs e)
    {
      log.WriteEntry(string.Format("Changed: {0}", e.FullPath), EventLogEntryType.Information);
    }

    public void Renamed(object sender, RenamedEventArgs e)
    {
      log.WriteEntry(string.Format("Renamed: {0} to {1}", e.OldFullPath, e.FullPath), EventLogEntryType.Information);
    }

    //

    private readonly EventLog log;
  }
}