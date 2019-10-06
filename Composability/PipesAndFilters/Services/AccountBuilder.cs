using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Services
{
    public class AccountBuilder : ISource<AccountDTO>
    {
        public AccountBuilder(params ISource<string>[] filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            if (filters.Length != 3)
                throw new ArgumentException("Unexpected number of filters.", nameof(filters));

            this.filters = filters;
        }

        public AccountDTO Process(Unit _)
        {
            return new AccountDTO
            {
                Email = filters[0].Process(Unit.INSTANCE),
                Phone = filters[1].Process(Unit.INSTANCE),
                Password = filters[2].Process(Unit.INSTANCE),
            };
        }

        //

        private readonly ISource<string>[] filters;
    }
}