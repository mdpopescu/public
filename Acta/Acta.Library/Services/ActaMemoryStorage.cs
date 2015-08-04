using System.Collections.Generic;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;

namespace Acta.Library.Services
{
  public class ActaMemoryStorage : IActaStorage
  {
    public void Append(ActaTuple tuple)
    {
      //
    }

    public IEnumerable<ActaTuple> Get()
    {
      return Enumerable.Empty<ActaTuple>();
    }
  }
}