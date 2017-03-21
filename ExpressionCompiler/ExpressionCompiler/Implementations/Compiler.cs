using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ExpressionCompiler.Models;

namespace ExpressionCompiler.Implementations
{
    public class Compiler
    {
        public int Eval(string expr)
        {
            var tokens = Tokenize(expr).ToList();

            return 0;
        }

        //

        private static readonly Regex WHITESPACE = new Regex(@"^\s+", RegexOptions.Compiled);

        private static readonly Regex INTEGER = new Regex(@"^\d+", RegexOptions.Compiled);

        private static readonly Regex OP_ADD = new Regex(@"^\+", RegexOptions.Compiled);
        private static readonly Regex OP_SUB = new Regex(@"^-", RegexOptions.Compiled);
        private static readonly Regex OP_MUL = new Regex(@"^\*", RegexOptions.Compiled);
        private static readonly Regex OP_DIV = new Regex(@"^/", RegexOptions.Compiled);

        private static readonly Dictionary<Regex, Func<string, Token>> MAP = new Dictionary<Regex, Func<string, Token>>
        {
            { WHITESPACE, _ => null },
            { INTEGER, s => new IntOperand(s) },
            { OP_ADD, _ => new OpAdd() },
            { OP_SUB, _ => new OpSub() },
            { OP_MUL, _ => new OpMul() },
            { OP_DIV, _ => new OpDiv() },
        };

        private static IEnumerable<Token> Tokenize(string expr)
        {
            while (expr != "")
            {
                var re = Identify(expr);
                if (re == null)
                    throw new Exception($"Cannot parse {expr}");

                var match = re.Match(expr);
                var matched = match.Groups[0].Value;

                var token = MAP[re].Invoke(matched);
                if (token != null)
                    yield return token;

                expr = expr.Substring(matched.Length);
            }
        }

        private static Regex Identify(string expr) => MAP.Keys.FirstOrDefault(re => re.IsMatch(expr));
    }
}