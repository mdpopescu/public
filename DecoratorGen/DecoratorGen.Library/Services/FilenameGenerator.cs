using System.Text.RegularExpressions;
using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class FilenameGenerator : IFilenameGenerator
    {
        public string Generate(string code)
        {
            var match = RE.Match(code);
            return match.Groups[1].Value + ".cs";
        }

        //

        private static readonly Regex RE = new Regex("class (\\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}