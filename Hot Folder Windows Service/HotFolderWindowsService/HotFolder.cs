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