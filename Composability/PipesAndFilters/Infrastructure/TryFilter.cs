using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Infrastructure
{
    public class TryFilter<TIn, TOut> : ITarget<TIn>
    {
        public TryFilter(IFilter<TIn, TOut> filter, ITarget<TOut> onSuccess, ITarget<Exception> onError)
        {
            this.filter = filter;
            this.onSuccess = onSuccess;
            this.onError = onError;
        }

        public Unit Process(TIn input)
        {
            try
            {
                var result = filter.Process(input);
                return onSuccess.Process(result);
            }
            catch (Exception ex)
            {
                return onError.Process(ex);
            }
        }

        //

        private readonly IFilter<TIn, TOut> filter;
        private readonly ITarget<TOut> onSuccess;
        private readonly ITarget<Exception> onError;
    }
}