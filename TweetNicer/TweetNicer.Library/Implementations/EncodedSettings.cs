using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class EncodedSettings : Storage<string, Settings>
    {
        public EncodedSettings(Storage<string, string> basicStorage, Encoder<string, Settings> encoder)
        {
            this.basicStorage = basicStorage;
            this.encoder = encoder;
        }

        public Settings LoadUserValues(string key)
        {
            return encoder.Decode(basicStorage.LoadUserValues(key));
        }

        public Settings LoadMachineValues(string key)
        {
            return encoder.Decode(basicStorage.LoadMachineValues(key));
        }

        public void SaveUserValues(string key, Settings settings)
        {
            basicStorage.SaveUserValues(key, encoder.Encode(settings));
        }

        public void SaveMachineValues(string key, Settings settings)
        {
            basicStorage.SaveMachineValues(key, encoder.Encode(settings));
        }

        //

        private readonly Storage<string, string> basicStorage;
        private readonly Encoder<string, Settings> encoder;
    }
}