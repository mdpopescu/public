using System;
using WebScraping.Library.Interfaces;

namespace WebScraping.Library.Implementations.StmtComp
{
    public class PrintCompiler : StatementCompiler
    {
        public bool CanHandle(string[] statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (statement.Length == 0)
                return false;

            var line = statement[0];
            var index = line.IndexOf("print", StringComparison.OrdinalIgnoreCase);
            return index == 0 && (line.Length == 5 || line[5] == ' ');
        }

        public string[] Compile(string[] statement)
        {
            var line = statement[0];

            var expr = line.Substring(6).Replace("'", "\"");
            return new[] { "output.WriteLine(" + expr + ");" };
        }
    }
}