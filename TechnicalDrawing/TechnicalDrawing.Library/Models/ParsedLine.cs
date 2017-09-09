namespace TechnicalDrawing.Library.Models
{
    public struct ParsedLine
    {
        public ParsedLine(string name, params float[] args)
        {
            Name = name;
            Args = args;
        }

        public string Name { get; }
        public float[] Args { get; }
    }
}