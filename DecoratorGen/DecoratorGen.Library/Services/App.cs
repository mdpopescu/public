using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class App
    {
        public App(IFileSystem fs, IGenerator generator)
        {
            this.fs = fs;
            this.generator = generator;
        }

        public void GenerateDecorator(string filename)
        {
            //
        }

        //

        private IFileSystem fs;
        private IGenerator generator;
    }
}