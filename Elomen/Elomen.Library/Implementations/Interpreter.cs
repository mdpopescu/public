using Elomen.Library.Contracts;

namespace Elomen.Library.Implementations
{
    public class Interpreter
    {
        public Interpreter(AccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="accountId">The account of the user sending the command.</param>
        /// <param name="command">The (natural language) command.</param>
        /// <returns>A (natural language) confirmation / rejection of the command.</returns>
        public string Execute(string accountId, string command)
        {
            accountRepository.Find(accountId);

            return null;
        }

        //

        private readonly AccountRepository accountRepository;
    }
}