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

            var statements = GetStatements(program);

            bool finished;
            do
            {
                finished = SingleStageCompile(ref statements);
            } while (!finished);

            return string.Format(TEMPLATE, string.Join(Environment.NewLine, statements.SelectMany(line => line)));
        }

        //

        private readonly StatementCompiler[] statementCompilers;

        private static IEnumerable<string[]> GetStatements(string program)
        {
            return program
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => new[] { line })
                .ToArray();
        }

        private bool SingleStageCompile(ref IEnumerable<string[]> statements)
        {
            var result = new List<string[]>();

            var finished = true;
            foreach (var statement in statements)
            {
                var compiler = statementCompilers.FirstOrDefault(it => it.CanHandle(statement));
                if (compiler == null)
                    result.Add(statement);
                else
                {
                    result.Add(compiler.Compile(statement));
                    finished = false;
                }
            }

            statements = result;
            return finished;
        }
    }
}