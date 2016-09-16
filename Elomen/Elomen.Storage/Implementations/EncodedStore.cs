using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    /// <summary>
    /// A resource store that does encoding when loading / saving.
    /// </summary>
    /// <typeparam name="TInternal">The internal type.</typeparam>
    /// <typeparam name="TExternal">The external type.</typeparam>
    public class EncodedStore<TInternal, TExternal> : ResourceStore<TInternal>
    {
        public EncodedStore(ResourceStore<TExternal> store, Encoder<TInternal, TExternal> encoder)
        {
            this.store = store;
            this.encoder = encoder;
        }

        public TInternal Load()
        {
            return encoder.Decode(store.Load());
        }

        public void Save(TInternal value)
        {
            store.Save(encoder.Encode(value));
        }

        //

        private readonly ResourceStore<TExternal> store;
        private readonly Encoder<TInternal, TExternal> encoder;
    }
}