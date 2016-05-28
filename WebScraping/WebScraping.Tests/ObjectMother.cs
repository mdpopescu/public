using System.IO;
using System.Text;
using WebScraping.Library.Models;

namespace WebScraping.Tests
{
    public static class ObjectMother
    {
        public static Environment CreateEnvironment(string input, StringBuilder output)
        {
            return new Environment(new StringReader(input), new StringWriter(output));
        }
    }
}