using System.Drawing;

namespace Turtles.Library.Models;

public record PolarCoords(double Angle, double Distance)
{
    public PointF ToCartesian()
    {
        var x = (float)(Math.Cos(Angle) * Distance);
        var y = (float)(Math.Sin(Angle) * Distance);
        return new PointF(x, y);
    }
}