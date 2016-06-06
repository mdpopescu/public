using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class FileSettings : SecureSettings<string>
    {
        public FileSettings(SecureStorage storage, Serializer<Settings> serializer)
        {
            this.storage = storage;
            this.serializer = serializer;
        }

        public Settings LoadUserSettings(string source)
        {
            return serializer.Deserialize(storage.LoadUserData(source));
        }

        public Settings LoadMachineSettings(string source, string password)
        {
            return serializer.Deserialize(storage.LoadMachineData(source, password));
        }

        public void SaveUserSettings(string destination, Settings settings)
        {
            storage.SaveUserData(destination, serializer.Serialize(settings));
        }

        public void SaveMachineSettings(string destination, string password, Settings settings)
        {
            storage.SaveMachineData(destination, password, serializer.Serialize(settings));
        }

        //

        private readonly SecureStorage storage;
        private readonly Serializer<Settings> serializer;
    }
}