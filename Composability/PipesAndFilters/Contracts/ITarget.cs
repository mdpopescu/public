using PipesAndFilters.Models;

namespace PipesAndFilters.Contracts
{
    public interface ITarget<in TIn> : IFilter<TIn, Unit>
    {
    }
}