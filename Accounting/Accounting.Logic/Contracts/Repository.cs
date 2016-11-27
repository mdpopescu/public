using System.Collections.Generic;

namespace Accounting.Logic.Contracts
{
    public interface Repository<T>
    {
        void Add(T value);
        IEnumerable<T> GetAll();
    }
}