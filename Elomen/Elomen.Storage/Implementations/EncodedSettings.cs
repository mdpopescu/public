using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class EncodedSettings : ResourceStore<CompositeSettings>
    {
        public EncodedSettings(ResourceStore<string> store, Encoder<CompositeSettings, string> encoder)
        {
            this.store = store;
            this.encoder = encoder;
        }

        public CompositeSettings Load()
        {
            return encoder.Decode(store.Load());
        }

        public void Save(CompositeSettings value)
        {
            store.Save(encoder.Encode(value));
        }

        //

        private readonly ResourceStore<string> store;
        private readonly Encoder<CompositeSettings, string> encoder;
    }
}