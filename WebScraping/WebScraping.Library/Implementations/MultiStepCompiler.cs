using System;
using System.Collections.Generic;
using System.Linq;
using WebScraping.Library.Interfaces;

namespace WebScraping.Library.Implementations
{
    public class MultiStepCompiler : Compiler
    {
        public MultiStepCompiler()
        {
            stages = new List<Func<string[], string[]>>
            {
                Stage1,
            };
        }

        public string Compile(string program)
        {
            const string TEMPLATE = @"public static class Program
{{
public void Main(TextReader input, TextWriter output)
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

        private readonly List<Func<string[], string[]>> stages;

        private static string[] Stage1(string[] lines)
        {
            var result = new List<string>();

            for (var i = 0; i < lines.Length; i++)
            {
                result.Add($"//{(i + 1)}//");

                var line = lines[i];
                if (line.IndexOf("print ", StringComparison.OrdinalIgnoreCase) != 0)
                    continue;

                var expr = line.Substring(6).Replace("'", "\"");
                line = "output.WriteLine(" + expr + ");";
                result.Add(line);
            }

            return result.ToArray();
        }
    }
}