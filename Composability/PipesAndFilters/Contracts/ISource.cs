using PipesAndFilters.Models;

namespace PipesAndFilters.Contracts
{
    public interface ISource<out TOut> : IFilter<Unit, TOut>
    {
    }
}