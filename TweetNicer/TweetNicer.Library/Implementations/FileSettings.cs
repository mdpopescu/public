using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class FileSettings : SecureSettings
    {
        public FileSettings(SecureStorage storage, Serializer<Settings> serializer)
        {
            this.storage = storage;
            this.serializer = serializer;
        }

        public Settings LoadUserSettings(string path)
        {
            return serializer.Deserialize(storage.LoadUserData(path));
        }

        public Settings LoadMachineSettings(string path, string password)
        {
            return serializer.Deserialize(storage.LoadMachineData(path, password));
        }

        public void SaveUserSettings(string path, Settings settings)
        {
            storage.SaveUserData(path, serializer.Serialize(settings));
        }

        public void SaveMachineSettings(string path, string password, Settings settings)
        {
            storage.SaveMachineData(path, password, serializer.Serialize(settings));
        }

        //

        private readonly SecureStorage storage;
        private readonly Serializer<Settings> serializer;
    }
}