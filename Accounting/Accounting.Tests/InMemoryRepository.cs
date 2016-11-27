using System.Collections.Generic;
using Accounting.Logic.Contracts;

namespace Accounting.Tests
{
    public class InMemoryRepository<T> : Repository<T>
    {
        public InMemoryRepository(IList<T> list)
        {
            this.list = list;
        }

        public void Add(T value)
        {
            list.Add(value);
        }

        public IEnumerable<T> GetAll()
        {
            return list;
        }

        //

        private readonly IList<T> list;
    }
}