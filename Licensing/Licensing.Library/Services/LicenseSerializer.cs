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
    public LicenseSerializer()
    {
      serializer = new XmlSerializer(typeof (LicenseRegistration));
    }

    public string Serialize(LicenseRegistration obj)
    {
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
      var settings = new XmlReaderSettings();

      using (var textReader = new StringReader(s))
      {
        using (var xmlReader = XmlReader.Create(textReader, settings))
        {
          return (LicenseRegistration) serializer.Deserialize(xmlReader);
        }
      }
    }

    //

    private readonly XmlSerializer serializer;
  }
}