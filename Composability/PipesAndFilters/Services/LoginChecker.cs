using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Services
{
    public class LoginChecker : IFilter<AccountDTO, bool>
    {
        public bool Process(AccountDTO account)
        {
            return account.Email == "a@b.c"
                && account.Phone == "123"
                && account.Password == "456";
        }
    }
}