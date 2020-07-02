using System.IO;
using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class FilenameGenerator : IFilenameGenerator
    {
        public string Generate(string filename)
        {
            var fileOnly = Path.GetFileNameWithoutExtension(filename);
            var firstTwoLetters = fileOnly.Substring(0, 2);
            var newFile = firstTwoLetters.Length == 2 && firstTwoLetters[0] == 'I' && char.IsUpper(firstTwoLetters[1])
                ? fileOnly.Substring(1)
                : fileOnly;
            var extension = Path.GetExtension(filename);

            return Path.Combine(Path.GetDirectoryName(filename) + "", $"{newFile}Decorator{extension}");
        }
    }
}