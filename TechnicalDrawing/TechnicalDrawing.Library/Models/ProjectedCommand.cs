namespace TechnicalDrawing.Library.Models
{
    public struct ProjectedCommand
    {
        public ProjectedCommand(Plane plane, CommandName name, params Point2D[] points)
        {
            Plane = plane;
            Name = name;
            Points = points;
        }

        public Plane Plane { get; }
        public CommandName Name { get; }
        public Point2D[] Points { get; }

        public override string ToString() => $"{Plane} {Name} {string.Join(", ", Points)}";
    }
}