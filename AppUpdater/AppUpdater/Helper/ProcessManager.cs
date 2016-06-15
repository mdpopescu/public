using System.Diagnostics;
using System.Windows.Forms;

namespace Renfield.AppUpdater.Helper
{
  internal class ProcessManager : System
  {
    #region Implementation of system

    public void Launch(string fileName)
    {
      var process = new Process { StartInfo = { FileName = fileName } };
      process.Start();
    }

    public void EndCurrentApplication()
    {
      Application.Exit();
    }

    #endregion
  }
}