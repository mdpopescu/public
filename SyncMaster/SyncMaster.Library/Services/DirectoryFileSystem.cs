using SyncMaster.Library.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncMaster.Library.Models;
using System.IO;

namespace SyncMaster.Library.Services
{
    public class DirectoryFileSystem : DirectoryRepository
    {
        public DirectoryInformation BuildDirectoryInfo(string path)
        {
            var result = new DirectoryInformation();
            result.FullPath = path;

            var files = Directory.GetFiles(path, "*.", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
                result.Files.Add(new FileInformation(file));

            var folders = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
                result.Directories.Add(BuildDirectoryInfo(folder));

            return result;
        }
    }
}
