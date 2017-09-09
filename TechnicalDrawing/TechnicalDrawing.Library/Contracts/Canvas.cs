using System.Collections.Generic;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Contracts
{
    public interface Canvas
    {
        void Execute(IEnumerable<ProjectedCommand> commands);
    }
}