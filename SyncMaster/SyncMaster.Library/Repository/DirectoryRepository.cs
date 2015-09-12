using SyncMaster.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMaster.Library.Repository
{
    public interface DirectoryRepository
    {

        DirectoryInformation BuildDirectoryInfo(string path);
    }
}
