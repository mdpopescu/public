using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class OutputWrapper<T>
    {
        public OutputWrapper(T value, string output)
        {
            this.value = value;
            this.output = output;
        }

        public T Unwrap(IWriter writer)
        {
            writer.WriteLine(output);
            return value;
        }

        //

        private readonly T value;
        private readonly string output;
    }

    public static class OutputWrapperExtensions
    {
        public static OutputWrapper<T> WithOutput<T>(this T value, string output) => new OutputWrapper<T>(value, output);
    }
}