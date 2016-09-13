using System;
using System.Collections.Generic;
using System.Linq;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class EnvironmentStorage : GenericStorage<IDictionary<string, string>>
    {
        public IDictionary<string, string> UserValues
        {
            get { return Load(EnvironmentVariableTarget.User); }
            set { Save(value, EnvironmentVariableTarget.User); }
        }

        public IDictionary<string, string> MachineValues
        {
            get { return Load(EnvironmentVariableTarget.Machine); }
            set { Save(value, EnvironmentVariableTarget.Machine); }
        }

        //

        private static IDictionary<string, string> Load(EnvironmentVariableTarget target)
        {
            var values = Environment.GetEnvironmentVariables(target);
            return values
                .Keys
                .Cast<string>()
                .ToDictionary(k => k, k => values[k] + "");
        }

        private static void Save(IDictionary<string, string> values, EnvironmentVariableTarget target)
        {
            foreach (var key in values.Keys)
                Environment.SetEnvironmentVariable(key, values[key], target);
        }
    }
}