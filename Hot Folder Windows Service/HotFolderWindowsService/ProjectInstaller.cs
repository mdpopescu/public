using System.ComponentModel;
using System.Configuration.Install;

namespace Renfield.HotFolderWindowsService
{
  [RunInstaller(true)]
  public partial class ProjectInstaller : Installer
  {
    public ProjectInstaller()
    {
      InitializeComponent();

      serviceInstaller.Installers.Clear();
    }
  }
}