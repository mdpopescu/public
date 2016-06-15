namespace Renfield.AppUpdater.Helper
{
  public interface ManifestReader
  {
    string Version { get; }
    string URL { get; }

    void Read(string manifest);
  }
}