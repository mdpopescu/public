using TechnicalDrawing.Library.Contracts;

namespace TechnicalDrawing.Library.Models
{
    public abstract class Command
    {
        public abstract void Execute(Canvas canvas);
    }
}