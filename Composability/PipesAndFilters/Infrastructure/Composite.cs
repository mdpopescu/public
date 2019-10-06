using PipesAndFilters.Contracts;

namespace PipesAndFilters.Infrastructure
{
    public class Composite<TIn, TTemp, TOut> : IFilter<TIn, TOut>
    {
        public Composite(IFilter<TIn, TTemp> f1, IFilter<TTemp, TOut> f2)
        {
            this.f1 = f1;
            this.f2 = f2;
        }

        public TOut Process(TIn input)
        {
            return f2.Process(f1.Process(input));
        }

        //

        private readonly IFilter<TIn, TTemp> f1;
        private readonly IFilter<TTemp, TOut> f2;
    }
}