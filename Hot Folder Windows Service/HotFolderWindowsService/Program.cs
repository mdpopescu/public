using System.ServiceProcess;

namespace Renfield.HotFolderWindowsService
{
  internal static class Program
  {
    private static void Main()
    {
      ServiceBase.Run(new HotFolder());
    }
  }
}