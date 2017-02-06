using System;
using System.Linq;
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

        private const char OPEN_PAR = '(';
        private const char CLOSED_PAR = ')';
        private const int DEPTH_BOOST = 10;

        private static readonly Operation[] OPERATIONS =
        {
            new Operation('+', 1, (a, b) => a + b),
            new Operation('-', 1, (a, b) => a - b),
            new Operation('*', 2, (a, b) => a * b),
            new Operation('/', 2, (a, b) => a / b),
        };

        private static string Eval(string expr)
        {
            var change = ReplaceSubexpr(expr);
            while (change.HasValue)
            {
                expr = change.Value;
                change = ReplaceSubexpr(expr);
            }

            return expr;
        }

        private static int GetDepthBoost(char c) => c == OPEN_PAR ? DEPTH_BOOST : c == CLOSED_PAR ? -DEPTH_BOOST : 0;
        private static Maybe<Operation> GetOperation(char c) => OPERATIONS.Where(op => op.Symbol == c).FirstMaybe();

        private static string Replace(string expr, int start, int end, string newValue) => expr.Substring(0, start) + newValue + expr.Substring(end + 1);

        private static Maybe<string> ReplaceSubexpr(string expr)
        {
            // todo: the parentheses mess up the replacement; get rid of them *after* computing the priority

            return from operation in FindMostImportantOperation(expr)
                   select IdentifySubexpr(expr, operation);
        }

        private static Maybe<OperationEx> FindMostImportantOperation(string expr)
        {
            var acc = 0;
            var depths = from c in expr
                         select acc += GetDepthBoost(c);

            // ensure that the parentheses are balanced
            if (acc != 0)
                return Maybe<OperationEx>.Nothing;

            // remove the parentheses
            var paired = expr
                .Zip(depths, Tuple.Create)
                .Where(tuple => tuple.Item1 != OPEN_PAR && tuple.Item1 != CLOSED_PAR);

            // note: do not inline the position variable or it won't be called for each character
            var index = 0;
            var operations = from pair in paired
                             let position = index++
                             let operation = from op in GetOperation(pair.Item1)
                                             select new OperationEx(position, pair.Item1, op.Priority + pair.Item2, op.Compute)
                             orderby operation.Select(it => it.Priority).OrElse(0) descending
                             where acc == 0
                             select operation;

            return operations.FirstMaybe();
        }

        private static Maybe<string> IdentifySubexpr(string expr, OperationEx operation)
        {
            var index1 = FindLeftOperandStart(expr, operation.Index);
            var index2 = FindRightOperandEnd(expr, operation.Index);
            var subexpr = expr.Substring(index1, index2 - index1 + 1);

            return from eval in EvalSubexpr(subexpr, operation, index1)
                   select Replace(expr, index1, index2, eval);
        }

        private static int FindLeftOperandStart(string expr, int index)
        {
            while (--index >= 0 && char.IsDigit(expr[index])) ;
            return index + 1;
        }

        private static int FindRightOperandEnd(string expr, int index)
        {
            while (++index < expr.Length && char.IsDigit(expr[index])) ;
            return index - 1;
        }

        private static Maybe<string> EvalSubexpr(string subexpr, OperationEx operation, int offset)
        {
            return from op1 in subexpr.Substring(0, operation.Index - offset).TryParse()
                   from op2 in subexpr.Substring(operation.Index - offset + 1).TryParse()
                   select Extensions.Safe(() => operation.Compute(op1, op2).ToString());
        }
    }
}