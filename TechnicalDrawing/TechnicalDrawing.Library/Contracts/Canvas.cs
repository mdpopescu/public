using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Contracts
{
    public interface Canvas
    {
        void Execute(ProjectedCommand command);
    }
}