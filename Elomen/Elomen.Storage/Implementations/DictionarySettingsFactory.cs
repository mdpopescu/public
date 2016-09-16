using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class DictionarySettingsFactory : CompositeSettingsFactory
    {
        public static readonly DictionarySettingsFactory INSTANCE = new DictionarySettingsFactory();

        public CompositeSettings Create()
        {
            return new DictionarySettings();
        }
    }
}