namespace TechnicalDrawing.Library.Models
{
    public struct ProjectedCommand
    {
        public ProjectedCommand(Plane plane, CommandName name, params QuadrantPoint[] points)
        {
            Plane = plane;
            Name = name;
            Points = points;
        }

        public Plane Plane { get; }
        public CommandName Name { get; }
        public QuadrantPoint[] Points { get; }

        public override string ToString() => $"{Plane} {Name} {string.Join(", ", Points)}";
    }
}