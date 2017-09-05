namespace SocialNetwork3.Library.Models
{
    public class ParsedLine
    {
        public ParsedLine(string user, string command, string rest)
        {
            User = user;
            Command = command;
            Rest = rest;
        }

        public string User { get; }
        public string Command { get; }
        public string Rest { get; }
    }
}