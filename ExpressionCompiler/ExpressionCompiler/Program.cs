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

        private const int DEPTH_BOOST = 10;

        private static readonly Operation[] OPERATIONS = { new Operation('+', 1), new Operation('-', 1), new Operation('*', 2), new Operation('/', 2), };

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

        private static int GetDepthBoost(char c) => c == '(' ? DEPTH_BOOST : c == ')' ? -DEPTH_BOOST : 0;
        private static Maybe<Operation> GetOperation(char c) => OPERATIONS.Where(op => op.Symbol == c).FirstMaybe();

        private static string Replace(string expr, int start, int end, string newValue) => expr.Substring(0, start) + newValue + expr.Substring(end + 1);

        private static Maybe<string> ReplaceSubexpr(string expr)
        {
            // todo: the parentheses mess up the replacement; get rid of them *after* computing the priority

            return from operation in FindMostImportantOperation(expr)
                   let op1 = FindLeftOperandStart(expr, operation.Index)
                   let op2 = FindRightOperandEnd(expr, operation.Index)
                   let subexpr = expr.Substring(op1, op2 - op1 + 1)
                   let result = from eval in EvalSubexpr(subexpr, operation)
                                select Replace(expr, op1, op2, eval)
                   select result;
        }

        private static Maybe<OperationEx> FindMostImportantOperation(string expr)
        {
            var acc = 0;
            var index = 0;

            // ensure that the parentheses are balanced (acc == 0)
            // note: do not inline the position variable or it won't be called for each character
            var operations = from c in expr
                             let position = index++
                             let depth = acc += GetDepthBoost(c)
                             let operation = from op in GetOperation(c)
                                             select new OperationEx(position, new Operation(c, op.Priority + depth))
                             orderby operation.Select(it => it.Priority).OrElse(0) descending
                             where acc == 0
                             select operation;

            return operations.FirstMaybe();
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

        private static Maybe<string> EvalSubexpr(string subexpr, OperationEx operation)
        {
            return from op1 in subexpr.Substring(0, operation.Index).TryParse()
                   from op2 in subexpr.Substring(operation.Index + 1).TryParse()
                   select Extensions.Safe(() => EvalSymbol(operation.Symbol, op1, op2).ToString());
        }

        private static int EvalSymbol(char symbol, int op1, int op2)
        {
            switch (symbol)
            {
                case '+':
                    return op1 + op2;

                case '-':
                    return op1 - op2;

                case '*':
                    return op1 * op2;

                case '/':
                    return op1 / op2;

                default:
                    throw new ArgumentException(nameof(symbol));
            }
        }
    }
}