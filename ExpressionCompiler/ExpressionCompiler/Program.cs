using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionCompiler.Contracts;
using ExpressionCompiler.Implementations;
using ExpressionCompiler.Models;
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
            var tokens = AdjustPriorities(LEXER.Parse(expr)).OrElse(() => new Exception("The parentheses are unbalanced."));

            foreach (var token in tokens)
                Console.WriteLine($"{token.Value} {token.GetType().Name}");

            return "";
        }

        private static Maybe<IEnumerable<Token>> AdjustPriorities(IEnumerable<Token> tokens)
        {
            var acc = 0;

            // must call ToList() here, otherwise nothing would be evaluated and acc will stay zero
            var result = (from token in tokens
                          let boost = acc += GetDepthBoost(token)
                          where !(token is PriorityBooster)
                          select BoostToken(token, boost))
                .ToList();

            // ensure that the parentheses are balanced
            return (acc == 0).Then(result.AsEnumerable);
        }

        private static int GetDepthBoost(Token token) => (token as PriorityBooster)?.Boost ?? 0;
        private static Token BoostToken(Token token, int boost) => (token as IBoostable)?.Boost(boost) ?? token;

        //private static string Eval(string expr)
        //{
        //    var change = ReplaceSubexpr(expr);
        //    while (change.HasValue)
        //    {
        //        expr = change.Value;
        //        change = ReplaceSubexpr(expr);
        //    }

        //    return expr;
        //}

        //private static int GetDepthBoost(char c) => c == OPEN_PAR ? DEPTH_BOOST : c == CLOSED_PAR ? -DEPTH_BOOST : 0;
        //private static Maybe<Operation> GetOperation(char c) => OPERATIONS.Where(op => op.Symbol == c).FirstMaybe();

        //private static Maybe<OperationEx> GetOperationEx(int position, char symbol, int priority)
        //    => GetOperation(symbol).Select(op => new OperationEx(position, symbol, op.Priority + priority, op.Compute));

        //private static string Replace(string expr, int start, int end, string newValue) => expr.Substring(0, start) + newValue + expr.Substring(end + 1);

        //private static Maybe<string> ReplaceSubexpr(string expr)
        //{
        //    return from operation in FindMostImportantOperation(expr)
        //           let expr1 = expr.Replace(OPEN_PAR.ToString(), "").Replace(CLOSED_PAR.ToString(), "")
        //           select ReplaceSubexprWithValue(expr1, operation);
        //}

        //private static Maybe<OperationEx> FindMostImportantOperation(string expr)
        //{
        //    return ComputeDepths(expr)
        //        .Select(depths => expr.Zip(depths, (symbol, priority) => new { symbol, priority }))
        //        .OrElseDefault()
        //        .Where(it => it.symbol != OPEN_PAR && it.symbol != CLOSED_PAR)
        //        .Select((it, position) => GetOperationEx(position, it.symbol, it.priority))
        //        .OrderByDescending(op => op.Select(it => it.Priority).OrElse(0))
        //        .FirstMaybe();
        //}

        //private static Maybe<IEnumerable<int>> ComputeDepths(string expr)
        //{
        //    var acc = 0;
        //    var depths = from c in expr
        //                 select acc += GetDepthBoost(c);

        //    // ensure that the parentheses are balanced
        //    return (acc == 0).Then(depths);
        //}

        //private static Maybe<string> ReplaceSubexprWithValue(string expr, OperationEx operation)
        //{
        //    var index1 = FindLeftOperandStart(expr, operation.Index);
        //    var index2 = FindRightOperandEnd(expr, operation.Index);
        //    var subexpr = expr.Substring(index1, index2 - index1 + 1);

        //    return from eval in EvalSubexpr(subexpr, operation, index1)
        //           select Replace(expr, index1, index2, eval);
        //}

        //private static int FindLeftOperandStart(string expr, int index)
        //{
        //    while (--index >= 0 && char.IsDigit(expr[index])) ;
        //    return index + 1;
        //}

        //private static int FindRightOperandEnd(string expr, int index)
        //{
        //    while (++index < expr.Length && char.IsDigit(expr[index])) ;
        //    return index - 1;
        //}

        //private static Maybe<string> EvalSubexpr(string subexpr, OperationEx operation, int offset)
        //{
        //    return from op1 in subexpr.Substring(0, operation.Index - offset).TryParse()
        //           from op2 in subexpr.Substring(operation.Index - offset + 1).TryParse()
        //           select Extensions.Safe(() => operation.Compute(op1, op2).ToString());
        //}
    }
}