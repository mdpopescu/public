using System;
using System.Xml.Linq;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class SettingsEncoder : Encoder<CompositeSettings, string>
    {
        public SettingsEncoder(Func<CompositeSettings> settingsFactory)
        {
            this.settingsFactory = settingsFactory;
        }

        public string Encode(CompositeSettings value)
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

        public CompositeSettings Decode(string data)
        {
            var result = settingsFactory.Invoke();

            var doc = XDocument.Parse(data);
            if (doc.Root == null)
                return result;

            foreach (var element in doc.Root.Elements())
            {
                var keyAttribute = element.Attribute("key");
                if (keyAttribute != null)
                    result[keyAttribute.Value] = element.Value;
            }

            return result;
        }

        //

        private readonly Func<CompositeSettings> settingsFactory;
    }
}