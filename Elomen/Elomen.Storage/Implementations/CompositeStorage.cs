using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class CompositeStorage : GenericStorage<CompositeSettings>
    {
        public CompositeStorage(string prefix, GenericStorage<IDictionary<string, string>> strings, Func<CompositeSettings> settingsFactory)
        {
            Debug.Assert(prefix != null, nameof(prefix));
            Debug.Assert(strings != null, nameof(strings));
            Debug.Assert(settingsFactory != null, nameof(settingsFactory));

            this.prefix = prefix;
            this.strings = strings;
            this.settingsFactory = settingsFactory;

            prefixLength = prefix.Length;
        }

        public CompositeSettings UserValues
        {
            get { return Load(strings.UserValues); }
            set { Save(value, strings.UserValues); }
        }

        public CompositeSettings MachineValues
        {
            get { return Load(strings.MachineValues); }
            set { Save(value, strings.MachineValues); }
        }

        //

        private readonly string prefix;
        private readonly GenericStorage<IDictionary<string, string>> strings;
        private readonly Func<CompositeSettings> settingsFactory;

        private readonly int prefixLength;

        private CompositeSettings Load(IDictionary<string, string> values)
        {
            var settings = settingsFactory.Invoke();

            var relevant = values
                .Keys
                .Where(it => it.StartsWith(prefix));
            foreach (var key in relevant)
                settings[key.Substring(prefixLength)] = values[key];

            return settings;
        }

        private void Save(CompositeSettings settings, IDictionary<string, string> values)
        {
            foreach (var key in settings.GetKeys())
                values[prefix + key] = settings[key];
        }
    }
}