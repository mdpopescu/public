using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class DictionarySettingsFactory : CompositeSettingsFactory
    {
        public CompositeSettings Create()
        {
            return new DictionarySettings();
        }
    }
}