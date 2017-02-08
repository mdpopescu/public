using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExpressionCompiler.Models;
using ExpressionCompiler.Models.Operands;
using ExpressionCompiler.Models.Operators;
using Functional.Maybe;

namespace ExpressionCompiler.Implementations
{
    public class Lexer
    {
        public IEnumerable<Token> Parse(string expr)
        {
            while (expr.Length > 0)
            {
                var next = (from match in IdentifyRegex(expr)
                            let re = match.Key
                            let func = match.Value
                            let value = re.Match(expr).Value
                            let token = func(value)
                            select new { token, skip = value.Length })
                    .OrElse(() => new Exception($"Cannot parse the expression [{expr}]"));

                expr = expr.Substring(next.skip);

                if (next.token != null)
                    yield return next.token;
            }
        }

        //

        private static readonly Regex INTEGER = new Regex(@"^\d+", RegexOptions.Compiled);
        private static readonly Regex OPAREN = new Regex(@"^\(", RegexOptions.Compiled);
        private static readonly Regex CPAREN = new Regex(@"^\)", RegexOptions.Compiled);
        private static readonly Regex PLUS = new Regex(@"^[+]", RegexOptions.Compiled);
        private static readonly Regex MINUS = new Regex(@"^[-]", RegexOptions.Compiled);
        private static readonly Regex MULTIPLY = new Regex(@"^[*]", RegexOptions.Compiled);
        private static readonly Regex DIVIDE = new Regex(@"^[/]", RegexOptions.Compiled);
        private static readonly Regex WHITESPACE = new Regex(@"^\s+", RegexOptions.Compiled);

        private static readonly Dictionary<Regex, Func<string, Token>> MAP = new Dictionary<Regex, Func<string, Token>>
        {
            { INTEGER, s => new Integer(s) },
            { OPAREN, _ => new OpenPar() },
            { CPAREN, _ => new ClosedPar() },
            { PLUS, _ => new IntegerAddition() },
            { MINUS, _ => new IntegerSubtraction() },
            { MULTIPLY, _ => new IntegerMultiplication() },
            { DIVIDE, _ => new IntegerDivision() },
            { WHITESPACE, _ => null },
        };

        private static Maybe<KeyValuePair<Regex, Func<string, Token>>> IdentifyRegex(string expr) => MAP.FirstMaybe(kv => kv.Key.IsMatch(expr));
    }
}