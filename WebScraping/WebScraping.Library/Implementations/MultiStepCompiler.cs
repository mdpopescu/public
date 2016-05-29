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

            stages = new List<Func<string[], string[]>>
            {
                Stage1,
            };
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
            var result = stages.Aggregate(lines, (current, stage) => stage(current));

            return string.Format(TEMPLATE, string.Join(Environment.NewLine, result));
        }

        //

        private readonly StatementCompiler[] statementCompilers;

        private readonly List<Func<string[], string[]>> stages;

        private string[] Stage1(string[] lines)
        {
            var result = new List<string>();

            for (var i = 0; i < lines.Length; i++)
            {
                result.Add($"//{(i + 1)}//");

                var line = lines[i];
                var statement = new[] { line };

                var compiler = statementCompilers.FirstOrDefault(it => it.CanHandle(statement));
                if (compiler == null)
                    continue;

                result.AddRange(compiler.Compile(statement));
            }

            return result.ToArray();
        }
    }
}