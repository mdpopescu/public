using System;
using System.Linq;
using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class EnvironmentStorage : Storage<string, Settings>
    {
        public EnvironmentStorage(Func<Settings> settingsFactory)
        {
            this.settingsFactory = settingsFactory;
        }

        public Settings LoadUserValues(string key)
        {
            return Load(key, EnvironmentVariableTarget.User);
        }

        public Settings LoadMachineValues(string key)
        {
            return Load(key, EnvironmentVariableTarget.Machine);
        }

        public void SaveUserValues(string key, Settings settings)
        {
            Save(key, settings, EnvironmentVariableTarget.User);
        }

        public void SaveMachineValues(string key, Settings settings)
        {
            Save(key, settings, EnvironmentVariableTarget.Machine);
        }

        //

        private readonly Func<Settings> settingsFactory;

        private Settings Load(string prefix, EnvironmentVariableTarget location)
        {
            var env = Environment.GetEnvironmentVariables(location);
            var relevant = env
                .Keys
                .Cast<string>()
                .Where(it => it.StartsWith(prefix));

            var skipCount = prefix.Length;

            var result = settingsFactory.Invoke();
            foreach (var v in relevant)
                result[v.Substring(skipCount)] = env[v] + "";

            return result;
        }

        private static void Save(string prefix, Settings settings, EnvironmentVariableTarget location)
        {
            foreach (var key in settings.GetKeys())
                Environment.SetEnvironmentVariable(prefix + key, settings[key], location);
        }
    }
}