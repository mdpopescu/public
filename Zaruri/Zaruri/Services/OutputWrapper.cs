namespace Zaruri.Services
{
    public class OutputWrapper<T>
    {
        public T Value { get; }
        public string Output { get; }

        public OutputWrapper(T value, string output)
        {
            Value = value;
            Output = output;
        }
    }

    public static class OutputWrapperExtensions
    {
        public static OutputWrapper<T> WithOutput<T>(this T value, string output) => new OutputWrapper<T>(value, output);
    }
}