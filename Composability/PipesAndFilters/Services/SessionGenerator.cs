using System;
using PipesAndFilters.Contracts;

namespace PipesAndFilters.Services
{
    public class SessionGenerator : IFilter<bool, string>
    {
        public string Process(bool input)
        {
            return input ? Guid.NewGuid().ToString("N") : throw new Exception("Invalid session.");
        }
    }
}