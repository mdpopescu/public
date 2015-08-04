using System;
using System.Collections.Generic;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;

namespace Acta.Library.Services
{
  public class ActaDb : IActaLowLevelApi
  {
    public ActaDb(IActaStorage storage)
    {
      //
    }

    public void Write(Guid guid, string name, object value)
    {
      //
    }

    public void Write(Guid guid, IEnumerable<ActaKeyValuePair> pairs)
    {
      //
    }

    public IEnumerable<Guid> GetIds(string name, object value)
    {
      return Enumerable.Empty<Guid>();
    }

    public object Read(Guid guid, string name)
    {
      return null;
    }
  }
}