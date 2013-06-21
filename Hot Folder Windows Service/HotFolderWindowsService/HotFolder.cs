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
      reporter = new Reporter(EventLog);
    }

    protected override void OnStart(string[] args)
    {
      fsw = new FileSystemWatcher(args[0]);
      fsw.Created += reporter.Created;
      fsw.Deleted += reporter.Deleted;
      fsw.Changed += reporter.Changed;
      fsw.Renamed += reporter.Renamed;

      EventLog.WriteEntry("Service started successfully.");
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

      EventLog.WriteEntry("Service stopped successfully.");
    }

    protected override void OnPause()
    {
      fsw.EnableRaisingEvents = false;

      EventLog.WriteEntry("Service paused.");
    }

    protected override void OnContinue()
    {
      fsw.EnableRaisingEvents = true;

      EventLog.WriteEntry("Service resumed.");
    }

    //

    private readonly Reporter reporter;
    private FileSystemWatcher fsw;
  }
}