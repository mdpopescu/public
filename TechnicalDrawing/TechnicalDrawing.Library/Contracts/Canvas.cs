using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Contracts
{
    public interface Canvas
    {
        void Line(Quadrant quadrant, ScreenPoint p1, ScreenPoint p2);
    }
}