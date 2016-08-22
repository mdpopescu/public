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

        public Settings LoadUserValues(string source)
        {
            return encoder.Decode(basicStorage.LoadUserValues(source));
        }

        public Settings LoadMachineValues(string source)
        {
            return encoder.Decode(basicStorage.LoadMachineValues(source));
        }

        public void SaveUserValues(string destination, Settings settings)
        {
            basicStorage.SaveUserValues(destination, encoder.Encode(settings));
        }

        public void SaveMachineValues(string destination, Settings settings)
        {
            basicStorage.SaveMachineValues(destination, encoder.Encode(settings));
        }

        //

        private readonly Storage<string, string> basicStorage;
        private readonly Encoder<string, Settings> encoder;
    }
}