using System.Diagnostics.CodeAnalysis;

namespace PipesAndFilters.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct AccountDTO
    {
        public string Email;
        public string Phone;
        public string Password;
    }
}