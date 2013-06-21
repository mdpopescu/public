using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace Renfield.HotFolderWindowsService
{
  public sealed partial class HotFolder : ServiceBase
  {
    public HotFolder()
    {
      InitializeComponent();

      EventLog.Log = ServiceName;
      EventLog.Source = ServiceName;

      reporter = new Reporter(EventLog);
    }

    protected override void OnStart(string[] args)
    {
      base.OnStart(args);

      var path = args[0];
      EventLog.WriteEntry("Monitoring " + path, EventLogEntryType.Information);

      fsw = new FileSystemWatcher(args[0])
      {
        Filter = "*.*",
        NotifyFilter = NotifyFilters.DirectoryName |
                       NotifyFilters.FileName |
                       NotifyFilters.LastWrite,
        IncludeSubdirectories = false,
      };
      fsw.Created += reporter.Created;
      fsw.Deleted += reporter.Deleted;
      fsw.Changed += reporter.Changed;
      fsw.Renamed += reporter.Renamed;

      fsw.EnableRaisingEvents = true;
    }

    protected override void OnStop()
    {
      fsw.EnableRaisingEvents = false;
      Thread.Sleep(50);

      fsw.Created -= reporter.Created;
      fsw.Deleted -= reporter.Deleted;
      fsw.Changed -= reporter.Changed;
      fsw.Renamed -= reporter.Renamed;

      fsw.Dispose();
      fsw = null;

      base.OnStop();
    }

    protected override void OnPause()
    {
      fsw.EnableRaisingEvents = false;
    }

    protected override void OnContinue()
    {
      fsw.EnableRaisingEvents = true;
    }

    //

    private readonly Reporter reporter;
    private FileSystemWatcher fsw;
  }
}