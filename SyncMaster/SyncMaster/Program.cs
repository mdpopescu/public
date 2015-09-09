using SyncMaster.Library.Models;
using SyncMaster.Library.Repository;
using SyncMaster.Library.Services;

namespace SyncMaster
{
  internal class Program
  {
    private static void Main(string[] args)
    {
       DirectoryRepository directoryRepository = new DirectoryFileSystem();
       string rootPath = "";
       foreach(var arg in args)
        {
            if (arg.StartsWith("--path="))
            {
                rootPath = arg.Replace("--path=", "");
            }
        }

        var rootFolder = directoryRepository.BuildDirectoryInfo(rootPath);
    }
  }
}