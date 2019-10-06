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
                    new ConsoleInput("Email:    "),
                    new ConsoleInput("Phone:    "),
                    new ConsoleInput("Password: ")
                ),
                new TryFilter<AccountDTO, string>(
                    new Composite<AccountDTO, bool, string>(
                        new LoginChecker(),
                        new SessionGenerator()
                    ),
                    new ConsoleOutput("Session ID: {0}."),
                    new ErrorOutput("ERROR: {0}")));
            var _ = program.Process(Unit.INSTANCE);
        }
    }
}