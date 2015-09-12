using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMaster.Library.Models
{
    public class FileInformation
    {
        public FileInformation() { }
        public FileInformation(string fullFileName)
        {
            this.FullFileName = fullFileName;
            this.LastWriteTime = File.GetLastWriteTime(fullFileName);
        }

        public string FullFileName { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
