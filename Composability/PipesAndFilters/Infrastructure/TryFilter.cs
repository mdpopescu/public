using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Infrastructure
{
    public class TryFilter<TIn, TOut> : IFilter<TIn, Unit>
    {
        public TryFilter(IFilter<TIn, TOut> filter, IFilter<TOut, Unit> onSuccess, IFilter<Exception, Unit> onError)
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
        private readonly IFilter<TOut, Unit> onSuccess;
        private readonly IFilter<Exception, Unit> onError;
    }
}