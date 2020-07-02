using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class App
    {
        public App(IFileSystem fs, ICodeGenerator codeGenerator, IFilenameGenerator filenameGenerator)
        {
            this.fs = fs;
            this.codeGenerator = codeGenerator;
            this.filenameGenerator = filenameGenerator;
        }

        public void GenerateDecorator(string filename)
        {
            var code = fs.ReadText(filename);
            var newCode = codeGenerator.Generate(code);
            var newFile = filenameGenerator.Generate(filename);
            fs.WriteText(newFile, newCode);
        }

        //

        private readonly IFileSystem fs;
        private readonly ICodeGenerator codeGenerator;
        private readonly IFilenameGenerator filenameGenerator;
    }
}