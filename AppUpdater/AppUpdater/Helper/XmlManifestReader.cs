using System.Xml.Linq;

namespace Renfield.AppUpdater.Helper
{
  internal class XmlManifestReader : ManifestReader
  {
    #region Implementation of ManifestReader

    public string Version { get; private set; }
    public string URL { get; private set; }

    public void Read(string manifest)
    {
      var xml = XDocument.Parse(manifest);

      Version = xml.Root.Element("version").Value;
      URL = xml.Root.Element("url").Value;
    }

    #endregion
  }
}