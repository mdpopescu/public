using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Models
{
    public class ParsedInput
    {
        public IUser User { get; set; }
        public string Command { get; set; }
        public string RestOfInput { get; set; }
    }
}