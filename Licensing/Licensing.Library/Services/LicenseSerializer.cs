using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class LicenseSerializer : Serializer<LicenseRegistration>
  {
    public string Serialize(LicenseRegistration obj)
    {
      var serializer = new XmlSerializer(typeof (LicenseRegistration));
      var settings = new XmlWriterSettings {Encoding = new UnicodeEncoding(false, false), Indent = false, OmitXmlDeclaration = false};

      using (var textWriter = new StringWriter())
      {
        using (var xmlWriter = XmlWriter.Create(textWriter, settings))
        {
          serializer.Serialize(xmlWriter, obj);
        }

        return textWriter.ToString();
      }
    }

    public LicenseRegistration Deserialize(string s)
    {
      var serializer = new XmlSerializer(typeof (LicenseRegistration));
      var settings = new XmlReaderSettings();

      using (var textReader = new StringReader(s))
      {
        using (var xmlReader = XmlReader.Create(textReader, settings))
        {
          return (LicenseRegistration) serializer.Deserialize(xmlReader);
        }
      }
    }
  }
}