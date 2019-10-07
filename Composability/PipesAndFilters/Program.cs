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
                new TryFilter<AccountDTO, string>(
                    new Composite<AccountDTO, bool, string>(
                        new LoginChecker(),
                        new SessionGenerator()
                    ),
                    new ConsoleWriter("Session ID: {0}."),
                    new ConsoleErrorWriter("ERROR: {0}")));
            var _ = program.Process(Unit.INSTANCE);
        }
    }
}