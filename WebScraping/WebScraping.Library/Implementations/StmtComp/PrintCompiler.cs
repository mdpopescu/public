using System;
using System.Diagnostics.Contracts;
using WebScraping.Library.Interfaces;

namespace WebScraping.Library.Implementations.StmtComp
{
    public class PrintCompiler : StatementCompiler
    {
        public bool CanHandle(string[] statement)
        {
            if (statement == null || statement.Length == 0)
                return false;

            var line = statement[0];
            var index = line.IndexOf("print", StringComparison.OrdinalIgnoreCase);
            return index == 0 && (line.Length == 5 || line[5] == ' ');
        }

        public string[] Compile(string[] statement)
        {
            Contract.Requires(statement != null && statement.Length > 0);

            var line = statement[0];

            var expr = line.Substring(6).Replace("'", "\"");
            return new[] { "output.WriteLine(" + expr + ");" };
        }
    }
}