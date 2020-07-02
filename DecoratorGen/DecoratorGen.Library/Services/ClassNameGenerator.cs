using System.Text.RegularExpressions;
using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class ClassNameGenerator : IClassNameGenerator
    {
        public string GenerateClassName(string interfaceCode)
        {
            var match = RE.Match(interfaceCode);
            var name = match.Groups[1].Value;
            var firstTwoLetters = name.Substring(0, 2);
            var newName = firstTwoLetters.Length == 2 && firstTwoLetters[0] == 'I' && char.IsUpper(firstTwoLetters[1])
                ? name.Substring(1)
                : name;

            return newName + "Decorator";
        }

        //

        private static readonly Regex RE = new Regex("interface\\s+(\\w+)\\s*\\{", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}