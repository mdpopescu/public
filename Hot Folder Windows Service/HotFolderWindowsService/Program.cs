using System.ServiceProcess;

namespace Renfield.HotFolderWindowsService
{
  internal static class Program
  {
    /// <summary>
    ///   The main entry point for the application.
    /// </summary>
    private static void Main()
    {
      var ServicesToRun = new ServiceBase[]
      {
        new HotFolder()
      };
      ServiceBase.Run(ServicesToRun);
    }
  }
}