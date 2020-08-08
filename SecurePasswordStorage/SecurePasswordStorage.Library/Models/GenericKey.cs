namespace SecurePasswordStorage.Library.Models
{
    public class GenericKey<T>
    {
        public T Value { get; }

        public GenericKey(T value)
        {
            Value = value;
        }
    }
}