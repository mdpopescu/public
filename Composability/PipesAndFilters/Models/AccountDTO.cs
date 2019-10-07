using System.Diagnostics.CodeAnalysis;

namespace PipesAndFilters.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class AccountDTO
    {
        public readonly string Email;
        public readonly string Phone;
        public readonly string Password;

        public AccountDTO(string email, string phone, string password)
        {
            Email = email;
            Phone = phone;
            Password = password;
        }
    }
}