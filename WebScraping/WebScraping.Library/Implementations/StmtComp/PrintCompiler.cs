using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using WebScraping.Library.Interfaces;

namespace WebScraping.Library.Implementations.StmtComp
{
    public class PrintCompiler : StatementCompiler
    {
        public bool CanHandle(string[] statement)
        {
            if (statement == null || statement.Length == 0)
                return false;

            var all = string.Join(" ", statement);
            return Regex.IsMatch(all, "^print(?:$|\\s)", RegexOptions.IgnoreCase);
        }

        public string[] Compile(string[] statement)
        {
            Contract.Requires(statement != null && statement.Length > 0);

            var all = string.Join(" ", statement);
            var expr = all.Substring(6).Replace("'", "\"");
            return new[] { "output.WriteLine(" + expr + ");" };
        }
    }
}