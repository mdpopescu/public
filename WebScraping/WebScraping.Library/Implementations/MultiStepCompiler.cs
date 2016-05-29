using System;
using System.Collections.Generic;
using System.Linq;
using WebScraping.Library.Interfaces;

namespace WebScraping.Library.Implementations
{
    public class MultiStepCompiler : Compiler
    {
        public MultiStepCompiler(params StatementCompiler[] statementCompilers)
        {
            this.statementCompilers = statementCompilers;
        }

        public string Compile(string program)
        {
            const string TEMPLATE = @"using System.IO;
public class Program
{{
public static void Main(TextReader input, TextWriter output)
{{
{0}
}}
}}
";

            if (string.IsNullOrWhiteSpace(program))
                return null;

            var lines = program
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            bool finished;
            do
            {
                finished = SingleStageCompile(ref lines);
            } while (!finished);

            return string.Format(TEMPLATE, string.Join(Environment.NewLine, lines));
        }

        //

        private readonly StatementCompiler[] statementCompilers;

        private bool SingleStageCompile(ref string[] lines)
        {
            var result = new List<string>();

            var finished = true;
            foreach (var line in lines)
            {
                // ignore comment lines
                if (line.StartsWith("//"))
                {
                    result.Add(line);
                    continue;
                }

                var statement = new[] { line };

                var compiler = statementCompilers.FirstOrDefault(it => it.CanHandle(statement));
                if (compiler != null)
                {
                    statement = compiler.Compile(statement);
                    finished = false;
                }

                result.AddRange(statement);
            }

            lines = result.ToArray();
            return finished;
        }
    }
}