using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMaster.Library.Models
{
    public class DirectoryInformation
    {
        public DirectoryInformation()
        {
            Files = new List<FileInformation>();
            Directories = new List<DirectoryInformation>();
        }
        
        public DateTime LastWriteTime { get; set; }
        public string FullPath { get; set; }
        public List<FileInformation> Files { get; set; }
        public List<DirectoryInformation> Directories { get; set; }
    }
}
