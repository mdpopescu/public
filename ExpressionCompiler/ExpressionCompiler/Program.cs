using System;
using ExpressionCompiler.Implementations;

namespace ExpressionCompiler
{
    internal class Program
    {
        private static void Main()
        {
            var compiler = new Compiler();

            do
            {
                Console.Write("Enter an expression: ");
                var expr = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(expr))
                    break;

                try
                {
                    Console.WriteLine($"Result: {compiler.Eval(expr)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error trying to evaluate the expression: {ex.Message}");
                }
            } while (true);
        }
    }
}