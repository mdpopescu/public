namespace PipesAndFilters.Contracts
{
    public interface IFilter<in TIn, out TOut>
    {
        TOut Process(TIn input);
    }
}