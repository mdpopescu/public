using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Infrastructure;
using PipesAndFilters.Models;
using PipesAndFilters.Services;

namespace PipesAndFilters
{
    internal class Program
    {
        private static void Main()
        {
            var program = new Composite<Unit, AccountDTO, Unit>(
                new AccountBuilder(
                    new ConsoleReader("Email:    "),
                    new ConsoleReader("Phone:    "),
                    new ConsoleReader("Password: ")
                ),
                new Composite<AccountDTO, IEffect, Unit>(
                    new Composite<AccountDTO, bool, IEffect>(
                        new LoginChecker(),
                        new SessionGenerator(
                            Console.Out,
                            Console.Error
                        )
                    ),
                    new Executor()
                )
            );
            var _ = program.Process(Unit.INSTANCE);
        }
    }
}