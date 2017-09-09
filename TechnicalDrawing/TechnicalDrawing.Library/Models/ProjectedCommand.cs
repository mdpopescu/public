namespace TechnicalDrawing.Library.Models
{
    public struct ProjectedCommand
    {
        public ProjectedCommand(CommandName name, Quadrant quadrant, params QuadrantPoint[] points)
        {
            Name = name;
            Quadrant = quadrant;
            Points = points;
        }

        public CommandName Name { get; }
        public Quadrant Quadrant { get; }
        public QuadrantPoint[] Points { get; }

        public override string ToString() => $"{Name} {Quadrant} {string.Join(", ", Points)}";
    }
}