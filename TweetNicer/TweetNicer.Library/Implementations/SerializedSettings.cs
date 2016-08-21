using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class SerializedSettings : SettingsStorage<string, Settings>
    {
        public SerializedSettings(SettingsStorage<string, string> basicStorage, Serializer<Settings> serializer)
        {
            this.basicStorage = basicStorage;
            this.serializer = serializer;
        }

        public Settings LoadUserSettings(string source)
        {
            return serializer.Deserialize(basicStorage.LoadUserSettings(source));
        }

        public Settings LoadMachineSettings(string source)
        {
            return serializer.Deserialize(basicStorage.LoadMachineSettings(source));
        }

        public void SaveUserSettings(string destination, Settings settings)
        {
            basicStorage.SaveUserSettings(destination, serializer.Serialize(settings));
        }

        public void SaveMachineSettings(string destination, Settings settings)
        {
            basicStorage.SaveMachineSettings(destination, serializer.Serialize(settings));
        }

        //

        private readonly SettingsStorage<string, string> basicStorage;
        private readonly Serializer<Settings> serializer;
    }
}