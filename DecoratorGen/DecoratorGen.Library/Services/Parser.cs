using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DecoratorGen.Library.Contracts;
using DecoratorGen.Library.Models;

namespace DecoratorGen.Library.Services
{
    public class Parser : IParser
    {
        public InterfaceData ExtractInterface(string code)
        {
            var lines = code
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .SelectMany(line => line.Split('\r', '\n'))
                .ToList();

            while (lines.Count > 0 && !RE1.IsMatch(lines[0]))
                lines.RemoveAt(0);

            // count the { and } characters until they match
            int? balance = null;
            for (var i = 0; i < lines.Count; i++)
            {
                var openCount = lines[i].Count(it => it == '{');
                var closedCount = lines[i].Count(it => it == '}');

                if (openCount > 0)
                    balance = balance.GetValueOrDefault() + openCount;
                if (closedCount > 0)
                    balance = balance.GetValueOrDefault() - closedCount;

                // if there's no { on the "interface ISomething" line, that doesn't mean they're balanced
                // we need to encounter at least one { or } before we can decide that
                if (balance.HasValue && balance.Value <= 0)
                    lines = lines.Take(i + 1).ToList();
            }

            var interfaceCode = string.Join(Environment.NewLine, lines);

            var match = RE2.Match(interfaceCode);
            var name = match.Groups[1].Value;

            return new InterfaceData { Name = name, Code = interfaceCode };
        }

        public IEnumerable<Member> ExtractMembers(string code) => Enumerable.Empty<Member>();

        //

        private static readonly Regex RE1 = new Regex("interface\\s+\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RE2 = new Regex("interface\\s+(\\w+)\\s*\\{", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}