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

        public IEnumerable<Member> ExtractMembers(string code) =>
            code
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ExtractPropertyOrMethod)
                .Where(it => it != null);

        //

        private static readonly Regex RE1 = new Regex("interface\\s+\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RE2 = new Regex("interface\\s+(\\w+)\\s*\\{", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RE3_RO = new Regex(
            "(\\w+)\\s+(\\w+)\\s*\\{\\s*get\\s*;\\s*\\}",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RE3_GENERAL = new Regex(
            "(\\w+)\\s+(\\w+)\\s*\\{\\s*(get\\s*;)?\\s*(set\\s*;)?\\s*\\}",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RE4 = new Regex("(\\w+)\\s+(\\w+)\\s*\\(([^)]*)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RE4_ARGS = new Regex("(\\w+)\\s+(\\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Member ExtractPropertyOrMethod(string line) =>
            ExtractProperty(line) ?? ExtractMethod(line);

        private static Member ExtractProperty(string line) =>
            ExtractReadOnlyProperty(line) ?? ExtractGeneralProperty(line);

        private static Member ExtractReadOnlyProperty(string line)
        {
            var match = RE3_RO.Match(line);
            return match.Success
                ? new ReadOnlyPropertyMember
                {
                    TypeName = match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                }
                : null;
        }

        private static Member ExtractGeneralProperty(string line)
        {
            var match = RE3_GENERAL.Match(line);
            return match.Success
                ? new PropertyMember
                {
                    TypeName = match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    HasGetter = match.Groups[3].Value != "",
                    HasSetter = match.Groups[4].Value != "",
                }
                : null;
        }

        private static Member ExtractMethod(string line)
        {
            var match = RE4.Match(line);
            return match.Success
                ? new MethodMember { TypeName = match.Groups[1].Value, Name = match.Groups[2].Value, Arguments = ExtractArguments(match.Groups[3].Value).ToArray() }
                : null;
        }

        private static IEnumerable<Argument> ExtractArguments(string args) =>
            args
                .Split(',')
                .Select(s => RE4_ARGS.Match(s))
                .Where(it => it.Success)
                .Select(match => new Argument { TypeName = match.Groups[1].Value, Name = match.Groups[2].Value });
    }
}