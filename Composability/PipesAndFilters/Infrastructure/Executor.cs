using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Infrastructure
{
    public class Executor : ITarget<IEffect>
    {
        public Unit Process(IEffect effect)
        {
            effect.Execute();
            return Unit.INSTANCE;
        }
    }
}