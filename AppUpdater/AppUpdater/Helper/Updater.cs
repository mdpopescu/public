namespace Renfield.AppUpdater.Helper
{
  public interface Updater
  {
    void CheckForUpdate(string uri, string currentVersion, ManifestReader reader);
    void Update(string destinationFolder, System system);
  }
}