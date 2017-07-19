using System;
using System.Collections.Generic;
using Acta.Library.Models;

namespace Acta.Library.Contracts
{
    public interface ActaEntityApi
    {
        Guid Store(object entity);

        IEnumerable<ActaKeyValuePair> Fetch(Guid id);
        T Fetch<T>(Guid id) where T : class, new();
    }
}