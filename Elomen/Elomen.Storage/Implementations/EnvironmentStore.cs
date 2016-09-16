﻿using System;
using System.Diagnostics;
using System.Linq;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    /// <summary>
    /// Stores data in the environment variables (user or machine).
    /// </summary>
    public class EnvironmentStore : ResourceStore<CompositeSettings>
    {
        public EnvironmentStore(EnvironmentVariableTarget target, string prefix, Func<CompositeSettings> settingsFactory)
        {
            Debug.Assert(!string.IsNullOrEmpty(prefix), nameof(prefix));

            this.target = target;
            this.prefix = prefix;
            this.settingsFactory = settingsFactory;

            prefixLength = prefix.Length;
        }

        public CompositeSettings Load()
        {
            var values = Environment.GetEnvironmentVariables(target);
            var keys = values
                .Keys
                .Cast<string>()
                .Where(it => it.StartsWith(prefix));

            var settings = settingsFactory.Invoke();
            foreach (var key in keys)
                settings[key.Substring(prefixLength)] = values[key] + "";

            return settings;
        }

        public void Save(CompositeSettings value)
        {
            foreach (var key in value.GetKeys())
                Environment.SetEnvironmentVariable(prefix + key, value[key], target);
        }

        //

        private readonly EnvironmentVariableTarget target;
        private readonly string prefix;
        private readonly Func<CompositeSettings> settingsFactory;

        private readonly int prefixLength;
    }
}