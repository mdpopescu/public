namespace TechnicalDrawing.Library.Models
{
    public struct ParsedCommand
    {
        public ParsedCommand(CommandName name, params float[] args)
        {
            Name = name;
            Args = args;
        }

        public CommandName Name { get; }
        public float[] Args { get; }
    }
}