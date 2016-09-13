using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class EncodedSettings : GenericStorage<CompositeSettings>
    {
        public EncodedSettings(GenericStorage<string> storage, Encoder<CompositeSettings, string> encoder)
        {
            this.storage = storage;
            this.encoder = encoder;
        }

        public CompositeSettings UserValues
        {
            get { return encoder.Decode(storage.UserValues); }
            set { storage.UserValues = encoder.Encode(value); }
        }

        public CompositeSettings MachineValues
        {
            get { return encoder.Decode(storage.MachineValues); }
            set { storage.MachineValues = encoder.Encode(value); }
        }

        //

        private readonly GenericStorage<string> storage;
        private readonly Encoder<CompositeSettings, string> encoder;
    }
}