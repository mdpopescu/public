using System.Collections.Generic;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Contracts
{
    public interface Canvas
    {
        /// <summary>Executes the specified commands.</summary>
        /// <param name="commands">The commands.</param>
        /// <remarks>All commands should have the same plane.</remarks>
        void Execute(IReadOnlyCollection<ProjectedCommand> commands);
    }
}