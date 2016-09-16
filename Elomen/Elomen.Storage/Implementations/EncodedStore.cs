using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class EncodedStore<T> : ResourceStore<T>
    {
        public EncodedStore(ResourceStore<T> store, Encoder<T, T> encoder)
        {
            this.store = store;
            this.encoder = encoder;
        }

        public T Load()
        {
            return encoder.Decode(store.Load());
        }

        public void Save(T value)
        {
            store.Save(encoder.Encode(value));
        }

        //

        private readonly ResourceStore<T> store;
        private readonly Encoder<T, T> encoder;
    }
}