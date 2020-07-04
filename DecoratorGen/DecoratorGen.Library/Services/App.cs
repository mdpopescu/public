using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Helpers;

namespace DecoratorGen.Library.Services
{
    public class App
    {
        public App(IFileSystem fs, ICodeGenerator codeGenerator, IOutput output)
        {
            this.fs = fs;
            this.codeGenerator = codeGenerator;
            this.output = output;
        }

        public void GenerateDecorator(string filename) =>
            filename
                .Pipe(fs.ReadText)
                .Pipe(codeGenerator.Generate)
                .Apply(output.WriteCode);

        //

        private readonly IFileSystem fs;
        private readonly ICodeGenerator codeGenerator;
        private readonly IOutput output;
    }
}