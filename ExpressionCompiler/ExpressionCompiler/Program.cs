using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionCompiler.Contracts;
using ExpressionCompiler.Implementations;
using ExpressionCompiler.Models;
using ExpressionCompiler.Models.Operands;
using ExpressionCompiler.Models.Operators;
using Functional.Maybe;

namespace ExpressionCompiler
{
    internal class Program
    {
        private static void Main()
        {
            do
            {
                Console.Write("Enter an expression: ");
                var expr = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(expr))
                    break;

                try
                {
                    Console.WriteLine($"Result: {Eval(expr)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error trying to evaluate the expression: {ex.Message}");
                }
            } while (true);
        }

        //

        private static readonly Lexer LEXER = new Lexer();

        private static string Eval(string expr)
        {
            var tokens = AdjustPriorities(LEXER.Parse(expr))
                .OrElse(() => new Exception("The parentheses are unbalanced."));

            while (tokens.Count > 1)
            {
                var index = FindMostImportantOperation(tokens);

                tokens = tokens.Left(index - 1)
                    .Concat(new[] { EvalOperation(tokens, index) })
                    .Concat(tokens.Mid(index + 2))
                    .ToList();
            }

            return tokens[0].Value;
        }

        private static Maybe<List<Token>> AdjustPriorities(IEnumerable<Token> tokens)
        {
            var acc = 0;

            // must call ToList() here, otherwise nothing would be evaluated and acc will stay zero
            var result = (from token in tokens
                          let boost = acc += GetDepthBoost(token)
                          where !(token is PriorityBooster)
                          select BoostToken(token, boost))
                .ToList();

            // ensure that the parentheses are balanced
            return (acc == 0).Then(result);
        }

        private static int GetDepthBoost(Token token) => (token as PriorityBooster)?.Boost ?? 0;
        private static Token BoostToken(Token token, int boost) => (token as IBoostable)?.Boost(boost) ?? token;

        private static int FindMostImportantOperation(IList<Token> tokens)
        {
            var index = -1;
            var priority = 0;

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i] as IntegerOperator;
                if (token == null || token.Priority <= priority)
                    continue;

                index = i;
                priority = token.Priority;
            }

            return index;
        }

        private static Token EvalOperation(IReadOnlyList<Token> tokens, int index)
        {
            var op = (IntegerOperator) tokens[index];

            // if the operator is unary, there might not be a left operand; default to zero
            var lOperand = (index > 0 ? tokens[index - 1] as Integer : null) ?? new Integer(0);
            var rOperand = (Integer) tokens[index + 1];

            return new Integer(op.Compute(lOperand.ActualValue, rOperand.ActualValue));
        }
    }
}