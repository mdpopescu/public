using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Helpers;

namespace DecoratorGen.Library.Services
{
    public class FileOutput : IOutput
    {
        public FileOutput(IFilenameGenerator filenameGenerator, IFileSystem fs)
        {
            this.filenameGenerator = filenameGenerator;
            this.fs = fs;
        }

        public void WriteCode(string code) =>
            code
                .Pipe(filenameGenerator.Generate)
                .Apply(filename => fs.WriteText(filename, code));

        //

        private readonly IFilenameGenerator filenameGenerator;
        private readonly IFileSystem fs;
    }
}