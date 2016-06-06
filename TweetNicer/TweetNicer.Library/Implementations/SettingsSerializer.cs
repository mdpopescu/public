using System;
using System.Xml.Linq;
using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class SettingsSerializer : Serializer<Settings>
    {
        public SettingsSerializer(Func<Settings> settingsFactory)
        {
            this.settingsFactory = settingsFactory;
        }

        public string Serialize(Settings value)
        {
            var doc = new XDocument(new XElement("settings"));

            foreach (var key in value.GetKeys())
            {
                var element = new XElement("setting");
                element.SetAttributeValue("key", key);
                element.SetValue(value[key]);

                // ReSharper disable once PossibleNullReferenceException
                doc.Root.Add(element);
            }

            return doc.ToString();
        }

        public Settings Deserialize(string data)
        {
            var result = settingsFactory.Invoke();

            var doc = XDocument.Parse(data);

            // ReSharper disable once PossibleNullReferenceException
            foreach (var element in doc.Root.Elements())
                result[element.Attribute("key").Value] = element.Value;

            return result;
        }

        //

        private readonly Func<Settings> settingsFactory;
    }
}